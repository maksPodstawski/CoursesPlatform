using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLLERS.Tests
{
    public class CoursesControllerTests
    {
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly Mock<ICreatorService> _mockCreatorService;
        private readonly Mock<IPurchasedCoursesService> _mockPurchasedCoursesService;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mockCourseService = new Mock<ICourseService>();
            _mockCreatorService = new Mock<ICreatorService>();
            _mockPurchasedCoursesService = new Mock<IPurchasedCoursesService>();

            _controller = new CoursesController(
                _mockCourseService.Object,
                _mockCreatorService.Object,
                _mockPurchasedCoursesService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private void SetUser(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task GetAllCourses_ReturnsOkWithCourses()
        {
            var courses = new List<Course> { new Course { Id = Guid.NewGuid(), Name = "Test" } };
            _mockCourseService.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(courses);

            var result = await _controller.GetAllCourses();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<CourseResponseDTO>>(okResult.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task GetCourseById_ExistingId_ReturnsOk()
        {
            var course = new Course { Id = Guid.NewGuid(), Name = "Test Course" };
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(course.Id)).ReturnsAsync(course);

            var result = await _controller.GetCourseById(course.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CourseResponseDTO>(okResult.Value);
            Assert.Equal(course.Id, returned.Id);
        }

        [Fact]
        public async Task GetCourseById_NonExisting_ReturnsNotFound()
        {
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Course)null);

            var result = await _controller.GetCourseById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCourseInstructor_Found_ReturnsOk()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };
            var creator = new Creator
            {
                Title = "Dr.",
                User = new User { UserName = "InstructorName" }
            };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
            _mockCreatorService.Setup(s => s.GetCreatorsByCourseAsync(courseId)).ReturnsAsync(new List<Creator> { creator });

            var result = await _controller.GetCourseInstructor(courseId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var instructor = Assert.IsType<InstructorResponseDTO>(okResult.Value);
            Assert.Equal("Dr.", instructor.Title);
        }

        [Fact]
        public async Task GetCourseInstructor_CourseNotFound_ReturnsNotFound()
        {
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Course)null);

            var result = await _controller.GetCourseInstructor(Guid.NewGuid());

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Course not found", notFound.Value);
        }

        [Fact]
        public async Task GetCourseInstructor_CreatorNotFound_ReturnsNotFound()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };
            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
            _mockCreatorService.Setup(s => s.GetCreatorsByCourseAsync(courseId)).ReturnsAsync(new List<Creator>());

            var result = await _controller.GetCourseInstructor(courseId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Course instructor not found", notFound.Value);
        }

        [Fact]
        public async Task GetCoursesByPriceRange_ReturnsOk()
        {
            var courses = new List<Course> { new Course { Price = 99.99M } };
            _mockCourseService.Setup(s => s.GetCoursesByPriceRangeAsync(50, 150)).ReturnsAsync(courses);

            var result = await _controller.GetCoursesByPriceRange(50, 150);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<CourseResponseDTO>>(okResult.Value);
            Assert.Single(returned);
        }

        [Fact]
        public async Task CreateCourse_Valid_ReturnsCreated()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var createDto = new CreateCourseDTO
            {
                Name = "Course 1",
                ImageUrl = "http://example.com/image.jpg",
                Duration = 120,
                Price = 100m
            };
            var createdCourse = Course.FromCreateDTO(createDto);
            createdCourse.Id = Guid.NewGuid();

            _mockCourseService.Setup(s => s.AddCourseAsync(It.IsAny<Course>())).ReturnsAsync(createdCourse);
            _mockCreatorService.Setup(s => s.AddCreatorFromUserAsync(Guid.Parse(userId), createdCourse.Id))
                .ReturnsAsync(new Creator());
            _mockPurchasedCoursesService.Setup(s => s.AddPurchasedCourseAsync(It.IsAny<PurchasedCourses>()))
                .ReturnsAsync(new PurchasedCourses());

            var result = await _controller.CreateCourse(createDto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<CourseResponseDTO>(created.Value);
            Assert.Equal(createdCourse.Id, returned.Id);
        }

        [Fact]
        public async Task UpdateCourse_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };
            var updateDto = new UpdateCourseDTO
            {
                Name = "Updated",
                Description = "Updated",
                ImageUrl = "url",
                Duration = 10,
                Price = 200
            };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(Guid.Parse(userId), courseId)).ReturnsAsync(true);
            _mockCourseService.Setup(s => s.UpdateCourseAsync(It.IsAny<Course>())).ReturnsAsync(course);

            var result = await _controller.UpdateCourse(courseId, updateDto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CourseResponseDTO>(ok.Value);
            Assert.Equal(course.Id, returned.Id);
        }

        [Fact]
        public async Task UpdateCourse_NotCreator_ReturnsForbid()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(Guid.Parse(userId), courseId)).ReturnsAsync(false);
            var updateDto = new UpdateCourseDTO
            {
                Name = "Updated Course Name",
                ImageUrl = "https://example.com/image.png",
                Duration = 120,
                Price = 199.99m,
                Description = "Optional description"
            };

            var result = await _controller.UpdateCourse(courseId, updateDto);

            Assert.IsType<ForbidResult>(result.Result);
        }
    }
}
