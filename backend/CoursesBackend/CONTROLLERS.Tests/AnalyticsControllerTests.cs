using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLLERS.Tests
{
    public class AnalyticsControllerTests
    {
        private readonly Mock<IAnalyticsService> _mockService;
        private readonly AnalyticsController _controller;
        private readonly Guid _userId;

        public AnalyticsControllerTests()
        {
            _mockService = new Mock<IAnalyticsService>();
            _controller = new AnalyticsController(_mockService.Object);

            _userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetMyAnalytics_ReturnsOk()
        {
            var expected = new CreatorAnalyticsDTO { CreatorId = _userId };
            _mockService.Setup(s => s.GetMyAnalyticsAsync(_userId, It.IsAny<int>())).ReturnsAsync(expected);

            var result = await _controller.GetMyAnalytics(null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetCreatorAnalytics_ReturnsOk()
        {
            var creatorId = Guid.NewGuid();
            var expected = new CreatorAnalyticsDTO { CreatorId = creatorId };
            _mockService.Setup(s => s.GetCreatorAnalyticsAsync(creatorId, It.IsAny<int>())).ReturnsAsync(expected);

            var result = await _controller.GetCreatorAnalytics(creatorId, null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
        }

        [Fact]
        public async Task GetCreatorAnalytics_ThrowsArgumentException_ReturnsNotFound()
        {
            var creatorId = Guid.NewGuid();
            _mockService.Setup(s => s.GetCreatorAnalyticsAsync(creatorId, It.IsAny<int>())).ThrowsAsync(new ArgumentException("Not found"));

            var result = await _controller.GetCreatorAnalytics(creatorId, null);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetMyCourseAnalytics_ReturnsOk()
        {
            var analytics = new CreatorAnalyticsDTO { CreatorId = _userId };
            var courses = new List<CourseAnalyticsDTO> { new CourseAnalyticsDTO { CourseId = Guid.NewGuid() } };
            _mockService.Setup(s => s.GetMyAnalyticsAsync(_userId)).ReturnsAsync(analytics);
            _mockService.Setup(s => s.GetCourseAnalyticsAsync(_userId)).ReturnsAsync(courses);

            var result = await _controller.GetMyCourseAnalytics();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(courses, okResult.Value);
        }

        [Fact]
        public async Task GetMonthlyRevenue_ReturnsOk()
        {
            var year = 2024;
            var analytics = new CreatorAnalyticsDTO { CreatorId = _userId };
            var revenue = new List<MonthlyRevenueDTO>();

            _mockService.Setup(s => s.GetMyAnalyticsAsync(_userId)).ReturnsAsync(analytics);
            _mockService.Setup(s => s.GetMonthlyRevenueAsync(_userId, year)).ReturnsAsync(revenue);

            var result = await _controller.GetMonthlyRevenue(year);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(revenue, okResult.Value);
        }

        [Fact]
        public async Task GetTopPerformingCourses_ReturnsOk()
        {
            var analytics = new CreatorAnalyticsDTO { CreatorId = _userId };
            var topCourses = new List<CoursePerformanceDTO> { new CoursePerformanceDTO { CourseId = Guid.NewGuid() } };

            _mockService.Setup(s => s.GetMyAnalyticsAsync(_userId)).ReturnsAsync(analytics);
            _mockService.Setup(s => s.GetTopPerformingCoursesAsync(_userId, 5)).ReturnsAsync(topCourses);

            var result = await _controller.GetTopPerformingCourses();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(topCourses, okResult.Value);
        }
    }
}
