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
    public class CoursePurchaseControllerTests
    {
        private readonly Mock<IPurchasedCoursesService> _mockPurchasedCoursesService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly CoursePurchaseController _controller;

        public CoursePurchaseControllerTests()
        {
            _mockPurchasedCoursesService = new Mock<IPurchasedCoursesService>();
            _mockCourseService = new Mock<ICourseService>();

            _controller = new CoursePurchaseController(
                _mockPurchasedCoursesService.Object,
                _mockCourseService.Object)
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
            var identity = new ClaimsIdentity(claims);
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task PurchaseCourse_NullDto_ReturnsBadRequest()
        {
            var result = await _controller.PurchaseCourse(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Purchase data is required", badRequest.Value);
        }

        [Fact]
        public async Task PurchaseCourse_UserNotAuthenticated_ReturnsUnauthorized()
        {
            var dto = new PurchaseCourseDTO { CourseId = Guid.NewGuid(), Price = 100 };

            var result = await _controller.PurchaseCourse(dto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task PurchaseCourse_CourseNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new PurchaseCourseDTO { CourseId = Guid.NewGuid(), Price = 100 };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(dto.CourseId))
                .ReturnsAsync((Course)null);

            var result = await _controller.PurchaseCourse(dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Course not found", notFound.Value);
        }

        [Fact]
        public async Task PurchaseCourse_AlreadyPurchased_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new PurchaseCourseDTO { CourseId = Guid.NewGuid(), Price = 100 };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(dto.CourseId))
                .ReturnsAsync(new Course());

            _mockPurchasedCoursesService.Setup(s =>
                s.HasUserPurchasedCourseAsync(Guid.Parse(userId), dto.CourseId))
                .ReturnsAsync(true);

            var result = await _controller.PurchaseCourse(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("You have already purchased this course", badRequest.Value);
        }

        [Fact]
        public async Task PurchaseCourse_AddFails_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new PurchaseCourseDTO { CourseId = Guid.NewGuid(), Price = 100 };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(dto.CourseId))
                .ReturnsAsync(new Course());

            _mockPurchasedCoursesService.Setup(s =>
                s.HasUserPurchasedCourseAsync(Guid.Parse(userId), dto.CourseId))
                .ReturnsAsync(false);

            _mockPurchasedCoursesService.Setup(s =>
                s.AddPurchasedCourseAsync(It.IsAny<PurchasedCourses>()))
                .ReturnsAsync((PurchasedCourses)null);

            var result = await _controller.PurchaseCourse(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Failed to create purchase record", badRequest.Value);
        }

        [Fact]
        public async Task PurchaseCourse_Valid_ReturnsCreated()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var dto = new PurchaseCourseDTO
            {
                CourseId = Guid.NewGuid(),
                Price = 150,
                ExpirationDate = DateTime.UtcNow.AddMonths(1)
            };

            var createdPurchase = PurchasedCourses.FromDTO(dto, Guid.Parse(userId));
            createdPurchase.Id = Guid.NewGuid();

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(dto.CourseId))
                .ReturnsAsync(new Course());

            _mockPurchasedCoursesService.Setup(s =>
                s.HasUserPurchasedCourseAsync(Guid.Parse(userId), dto.CourseId))
                .ReturnsAsync(false);

            _mockPurchasedCoursesService.Setup(s =>
                s.AddPurchasedCourseAsync(It.IsAny<PurchasedCourses>()))
                .ReturnsAsync(createdPurchase);

            var result = await _controller.PurchaseCourse(dto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<PurchaseCourseResponseDTO>(createdAt.Value);
            Assert.Equal(createdPurchase.Id, returned.Id);
        }
    }
}
