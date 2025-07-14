using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLLERS.Tests
{
    public class StatisticsControllerTests
    {
        private readonly Mock<IUserService> _userService;
        private readonly Mock<ICourseService> _courseService;
        private readonly Mock<IReviewService> _reviewService;
        private readonly StatisticsController _controller;

        public StatisticsControllerTests()
        {
            _userService = new Mock<IUserService>();
            _courseService = new Mock<ICourseService>();
            _reviewService = new Mock<IReviewService>();

            _controller = new StatisticsController(
                _userService.Object,
                _courseService.Object,
                _reviewService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetStatistics_ReturnsCorrectCounts()
        {
            var users = new List<User> { new User(), new User() };
            var courses = new List<Course> { new Course() };
            var reviews = new List<Review> { new Review(), new Review(), new Review() };

            _userService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);
            _courseService.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(courses);
            _reviewService.Setup(s => s.GetAllReviewsAsync()).ReturnsAsync(reviews);

            var result = await _controller.GetStatistics();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!;
            var usersCount = (int)value.GetType().GetProperty("usersCount")!.GetValue(value)!;
            var coursesCount = (int)value.GetType().GetProperty("coursesCount")!.GetValue(value)!;
            var reviewsCount = (int)value.GetType().GetProperty("reviewsCount")!.GetValue(value)!;

            Assert.Equal(2, usersCount);
            Assert.Equal(1, coursesCount);
            Assert.Equal(3, reviewsCount);
        }

        [Fact]
        public async Task GetStatistics_EmptyLists_ReturnsZeroCounts()
        {
            _userService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<User>());
            _courseService.Setup(s => s.GetAllCoursesAsync()).ReturnsAsync(new List<Course>());
            _reviewService.Setup(s => s.GetAllReviewsAsync()).ReturnsAsync(new List<Review>());

            var result = await _controller.GetStatistics();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value!;
            var usersCount = (int)value.GetType().GetProperty("usersCount")!.GetValue(value)!;
            var coursesCount = (int)value.GetType().GetProperty("coursesCount")!.GetValue(value)!;
            var reviewsCount = (int)value.GetType().GetProperty("reviewsCount")!.GetValue(value)!;

            Assert.Equal(0, usersCount);
            Assert.Equal(0, coursesCount);
            Assert.Equal(0, reviewsCount);
        }

    }
}
