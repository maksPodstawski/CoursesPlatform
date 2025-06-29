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

namespace Controllers.Tests
{
    public class CreatorControllerTests
    {
        private readonly Mock<ICreatorService> _mockCreatorService;
        private readonly CreatorController _controller;

        public CreatorControllerTests()
        {
            _mockCreatorService = new Mock<ICreatorService>();

            _controller = new CreatorController(_mockCreatorService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private void SetUser(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetAllCreators_ReturnsOkWithCreators()
        {
            var creatorDtos = new List<CreatorResponseDTO>
            {
                new CreatorResponseDTO
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    UserName = "User1",
                    CoursesCount = 1,
                    CourseNames = new List<string> { "Course1" }
                },
                new CreatorResponseDTO
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    UserName = "User2",
                    CoursesCount = 2,
                    CourseNames = new List<string> { "Course2", "Course3" }
                }
            };
            var creators = creatorDtos.Select(dto => new Creator
            {
                Id = dto.Id,
                UserId = dto.UserId,
                User = new User { Id = dto.UserId, UserName = dto.UserName },
                Courses = dto.CourseNames.Select(name => new Course { Name = name }).ToList()
            }).ToList();

            _mockCreatorService.Setup(s => s.GetAllCreatorsAsync()).ReturnsAsync(creators);

            var result = await _controller.GetAllCreators();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<CreatorResponseDTO>>(okResult.Value);
            Assert.Equal(creatorDtos.Count, returned.Count());
        }

        [Fact]
        public async Task GetMyCreatorProfile_UserNotAuthenticated_ReturnsUnauthorized()
        {
            // No user set
            var result = await _controller.GetMyCreatorProfile();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetMyCreatorProfile_NoCourses_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            _mockCreatorService.Setup(s => s.GetCoursesByCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Course>());

            var result = await _controller.GetMyCreatorProfile();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("You are not a creator yet", notFound.Value);
        }

        [Fact]
        public async Task GetMyCreatorProfile_CreatorNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var course = new Course
            {
                Creators = new List<Creator> { new Creator { Id = Guid.NewGuid() } }
            };
            _mockCreatorService.Setup(s => s.GetCoursesByCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Course> { course });

            _mockCreatorService.Setup(s => s.GetCreatorByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Creator)null);

            var result = await _controller.GetMyCreatorProfile();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Creator profile not found", notFound.Value);
        }

        [Fact]
        public async Task GetMyCreatorProfile_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var creatorId = Guid.NewGuid();
            var course = new Course
            {
                Creators = new List<Creator> { new Creator { Id = creatorId } }
            };
            var creator = new Creator { Id = creatorId, UserId = Guid.Parse(userId) };

            _mockCreatorService.Setup(s => s.GetCoursesByCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Course> { course });

            _mockCreatorService.Setup(s => s.GetCreatorByIdAsync(creatorId))
                .ReturnsAsync(creator);

            var result = await _controller.GetMyCreatorProfile();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CreatorResponseDTO>(okResult.Value);
            Assert.Equal(creator.Id, returned.Id);
        }

        [Fact]
        public async Task BecomeCreator_UserNotAuthenticated_ReturnsUnauthorized()
        {
            var appendCreatorDto = new AppendCreatorDTO
            {
                CourseId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var result = await _controller.BecomeCreator(appendCreatorDto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task BecomeCreator_AlreadyCreator_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new AppendCreatorDTO { UserId = Guid.Parse(userId), CourseId = Guid.NewGuid() };

            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var result = await _controller.BecomeCreator(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("You are already a creator of this course", badRequest.Value);
        }

        [Fact]
        public async Task BecomeCreator_AddCreatorSuccess_ReturnsOk()
        {
            var userGuid = Guid.NewGuid();
            SetUser(userGuid.ToString());

            var dto = new AppendCreatorDTO
            {
                UserId = userGuid,
                CourseId = Guid.NewGuid()
            };

            var creatorDto = new CreatorResponseDTO
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                UserName = "TestUser",
                CoursesCount = 0,
                CourseNames = new List<string>()
            };
            var creator = creatorDto.ToCreator();

            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(userGuid, dto.CourseId))
                .ReturnsAsync(true);

            _mockCreatorService.Setup(s => s.AddCreatorFromUserAsync(dto.UserId, dto.CourseId))
                .ReturnsAsync(creator);

            var result = await _controller.BecomeCreator(dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CreatorResponseDTO>(okResult.Value);

            Assert.Equal(creatorDto.Id, returned.Id);
            Assert.Equal(creatorDto.UserId, returned.UserId);
            Assert.Equal(creatorDto.UserName, returned.UserName);
        }

        [Fact]
        public async Task BecomeCreator_CourseNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new AppendCreatorDTO { UserId = Guid.Parse(userId), CourseId = Guid.NewGuid() };

            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _mockCreatorService.Setup(s => s.AddCreatorFromUserAsync(dto.UserId, dto.CourseId))
                .ThrowsAsync(new ArgumentNullException());

            var result = await _controller.BecomeCreator(dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Course not found", notFound.Value);
        }

        [Fact]
        public async Task BecomeCreator_OtherException_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new AppendCreatorDTO { UserId = Guid.Parse(userId), CourseId = Guid.NewGuid() };

            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _mockCreatorService.Setup(s => s.AddCreatorFromUserAsync(dto.UserId, dto.CourseId))
                .ThrowsAsync(new Exception("Some error"));

            var result = await _controller.BecomeCreator(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Some error", badRequest.Value);
        }

        [Fact]
        public async Task GetMyCourses_UserNotAuthenticated_ReturnsUnauthorized()
        {
            var result = await _controller.GetMyCourses();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetMyCourses_NoCourses_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            _mockCreatorService.Setup(s => s.GetCoursesByCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Course>());

            var result = await _controller.GetMyCourses();

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("You don't have any courses yet", notFound.Value);
        }

        [Fact]
        public async Task GetMyCourses_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1", Price = 100m, Duration = 60, ImageUrl = "url1" },
                new Course { Id = Guid.NewGuid(), Name = "Course 2", Price = 200m, Duration = 120, ImageUrl = "url2" }
            };

            _mockCreatorService.Setup(s => s.GetCoursesByCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(courses);

            var result = await _controller.GetMyCourses();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<CourseResponseDTO>>(okResult.Value);
            Assert.Equal(courses.Count, returned.Count());
        }
    }
}
