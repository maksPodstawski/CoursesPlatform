using BL.Exceptions;
using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Controllers.Tests
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly ReviewController _controller;

        public ReviewControllerTests()
        {
            _mockReviewService = new Mock<IReviewService>();
            _mockCourseService = new Mock<ICourseService>();

            _controller = new ReviewController(_mockReviewService.Object, _mockCourseService.Object)
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
        public async Task GetAllReviews_ReturnsOkWithReviews()
        {
            var reviews = new List<Review> { CreateSampleReview(), CreateSampleReview() };
            _mockReviewService.Setup(s => s.GetAllReviewsAsync()).ReturnsAsync(reviews);

            var result = await _controller.GetAllReviews();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<ReviewResponseDTO>>(okResult.Value);
            Assert.Equal(reviews.Count, returned.Count());
        }

        [Fact]
        public async Task GetReviewById_ExistingId_ReturnsOk()
        {
            var review = CreateSampleReview();
            _mockReviewService.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await _controller.GetReviewById(review.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ReviewResponseDTO>(okResult.Value);
            Assert.Equal(review.Id, returned.Id);
        }

        [Fact]
        public async Task GetReviewById_NonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            _mockReviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync((Review)null);

            var result = await _controller.GetReviewById(id);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateReview_Valid_ReturnsCreated()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var courseId = Guid.NewGuid();
            var createDto = new CreateReviewDTO { CourseId = courseId, Rating = 5, Comment = "Good" };
            var createdReview = Review.FromCreateDTO(createDto, Guid.Parse(userId));
            createdReview.Id = Guid.NewGuid();

            createdReview.User = new User { Id = createdReview.UserId, UserName = "Test User" };
            createdReview.Course = new Course { Id = courseId, Name = "Test Course" };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course());
            _mockReviewService.Setup(s => s.GetReviewsByUserIdAsync(Guid.Parse(userId))).ReturnsAsync(new List<Review>());
            _mockReviewService.Setup(s => s.AddReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            var result = await _controller.CreateReview(createDto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<ReviewResponseDTO>(createdAt.Value);
            Assert.Equal(createdReview.Id, returned.Id);
        }

        [Fact]
        public async Task CreateReview_CourseNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var createDto = new CreateReviewDTO { CourseId = Guid.NewGuid(), Rating = 5, Comment = "Test" };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(createDto.CourseId)).ReturnsAsync((Course)null);

            var result = await _controller.CreateReview(createDto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Course not found", notFound.Value);
        }

        [Fact]
        public async Task CreateReview_AlreadyReviewed_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var courseId = Guid.NewGuid();
            var createDto = new CreateReviewDTO { CourseId = courseId, Rating = 4, Comment = "Nice" };

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course());
            _mockReviewService.Setup(s => s.GetReviewsByUserIdAsync(Guid.Parse(userId)))
                .ReturnsAsync(new List<Review> { CreateSampleReview(courseId, Guid.Parse(userId)) });

            var result = await _controller.CreateReview(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("You have already reviewed this course", badRequest.Value);
        }

        [Fact]
        public async Task UpdateReview_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var review = CreateSampleReview(userId: userId);
            var updateDto = new UpdateReviewDTO { Rating = 4, Comment = "Updated comment" };

            _mockReviewService.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);
            _mockReviewService.Setup(s => s.UpdateReviewAsync(review)).ReturnsAsync(review);

            var result = await _controller.UpdateReview(review.Id, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ReviewResponseDTO>(okResult.Value);
            Assert.Equal(review.Id, returned.Id);
        }

        [Fact]
        public async Task UpdateReview_NonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            SetUser(Guid.NewGuid().ToString());

            _mockReviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync((Review)null);

            var result = await _controller.UpdateReview(id, new UpdateReviewDTO { Rating = 3, Comment = "Test" });

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateReview_UserMismatch_ReturnsForbid()
        {
            var review = CreateSampleReview(userId: Guid.NewGuid());
            SetUser(Guid.NewGuid().ToString()); // Inny user

            _mockReviewService.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await _controller.UpdateReview(review.Id, new UpdateReviewDTO { Rating = 5, Comment = "xxx" });

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task DeleteReview_Valid_ReturnsNoContent()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var review = CreateSampleReview(userId: userId);
            _mockReviewService.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);
            _mockReviewService.Setup(s => s.DeleteReviewAsync(review.Id)).ReturnsAsync(review);

            var result = await _controller.DeleteReview(review.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteReview_NonExisting_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            SetUser(Guid.NewGuid().ToString());

            _mockReviewService.Setup(s => s.GetReviewByIdAsync(id)).ReturnsAsync((Review)null);

            var result = await _controller.DeleteReview(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteReview_UserMismatch_ReturnsForbid()
        {
            var review = CreateSampleReview(userId: Guid.NewGuid());
            SetUser(Guid.NewGuid().ToString()); // Inny user

            _mockReviewService.Setup(s => s.GetReviewByIdAsync(review.Id)).ReturnsAsync(review);

            var result = await _controller.DeleteReview(review.Id);

            Assert.IsType<ForbidResult>(result);
        }

        private Review CreateSampleReview(Guid? courseId = null, Guid? userId = null)
        {
            var createDto = new CreateReviewDTO
            {
                CourseId = courseId ?? Guid.NewGuid(),
                Rating = 5,
                Comment = "Sample comment"
            };

            var review = Review.FromCreateDTO(createDto, userId ?? Guid.NewGuid());
            review.Id = Guid.NewGuid();
            review.Course = new Course { Name = "Test Course" };
            review.User = new User { Id = review.UserId, UserName = "Test User" };
            return review;
        }
    }
}
