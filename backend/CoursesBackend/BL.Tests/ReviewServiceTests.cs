using BL.Services;
using IDAL;
using MockQueryable.Moq;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Tests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _mockReviewRepository;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _mockReviewRepository = new Mock<IReviewRepository>();
            _reviewService = new ReviewService(_mockReviewRepository.Object);
        }

        [Fact]
        public async Task GetAllReviewsAsync_ReturnsAllReviews()
        {
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 5, Comment = "Excellent", CourseId = Guid.NewGuid(), UserId = Guid.NewGuid() },
                new Review { Id = Guid.NewGuid(), Rating = 4, Comment = "Good", CourseId = Guid.NewGuid(), UserId = Guid.NewGuid() }
            };

            var mockDbSet = reviews.AsQueryable().BuildMockDbSet();
            _mockReviewRepository.Setup(r => r.GetReviews()).Returns(mockDbSet.Object);

            var result = await _reviewService.GetAllReviewsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetReviewByIdAsync_ExistingId_ReturnsReview()
        {
            var reviewId = Guid.NewGuid();
            var review = new Review { Id = reviewId, Rating = 3, Comment = "OK" };

            _mockReviewRepository.Setup(r => r.GetReviewById(reviewId)).Returns(review);

            var result = await _reviewService.GetReviewByIdAsync(reviewId);

            Assert.NotNull(result);
            Assert.Equal(3, result.Rating);
        }

        [Fact]
        public async Task GetReviewByIdAsync_NonExistingId_ReturnsNull()
        {
            var reviewId = Guid.NewGuid();
            _mockReviewRepository.Setup(r => r.GetReviewById(reviewId)).Returns((Review)null);

            var result = await _reviewService.GetReviewByIdAsync(reviewId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetReviewsByCourseIdAsync_ReturnsCorrectReviews()
        {
            var courseId = Guid.NewGuid();
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), CourseId = courseId },
                new Review { Id = Guid.NewGuid(), CourseId = courseId },
                new Review { Id = Guid.NewGuid(), CourseId = Guid.NewGuid() }
            };

            var mockDbSet = reviews.AsQueryable().BuildMockDbSet();
            _mockReviewRepository.Setup(r => r.GetReviews()).Returns(mockDbSet.Object);

            var result = await _reviewService.GetReviewsByCourseIdAsync(courseId);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(courseId, r.CourseId));
        }

        [Fact]
        public async Task GetReviewsByUserIdAsync_ReturnsCorrectReviews()
        {
            var userId = Guid.NewGuid();
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), UserId = userId },
                new Review { Id = Guid.NewGuid(), UserId = userId },
                new Review { Id = Guid.NewGuid(), UserId = Guid.NewGuid() }
            };

            var mockDbSet = reviews.AsQueryable().BuildMockDbSet();
            _mockReviewRepository.Setup(r => r.GetReviews()).Returns(mockDbSet.Object);

            var result = await _reviewService.GetReviewsByUserIdAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(userId, r.UserId));
        }

        [Fact]
        public async Task AddReviewAsync_AssignsIdAndAddsReview()
        {
            var review = new Review
            {
                Rating = 4,
                Comment = "Very good",
                CourseId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _mockReviewRepository.Setup(r => r.AddReview(It.IsAny<Review>())).Returns((Review r) => r);

            var result = await _reviewService.AddReviewAsync(review);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Very good", result.Comment);
        }

        [Fact]
        public async Task UpdateReviewAsync_UpdatesAndReturnsReview()
        {
            var review = new Review
            {
                Id = Guid.NewGuid(),
                Rating = 5,
                Comment = "Updated"
            };

            _mockReviewRepository.Setup(r => r.AddReview(review)).Returns(review); 

            var result = await _reviewService.UpdateReviewAsync(review);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Comment);
        }

        [Fact]
        public async Task DeleteReviewAsync_ExistingId_DeletesAndReturnsReview()
        {
            var reviewId = Guid.NewGuid();
            var review = new Review { Id = reviewId, Comment = "To delete" };

            _mockReviewRepository.Setup(r => r.DeleteReview(reviewId)).Returns(review);

            var result = await _reviewService.DeleteReviewAsync(reviewId);

            Assert.NotNull(result);
            Assert.Equal("To delete", result.Comment);
        }

        [Fact]
        public async Task DeleteReviewAsync_NonExistingId_ReturnsNull()
        {
            var reviewId = Guid.NewGuid();
            _mockReviewRepository.Setup(r => r.DeleteReview(reviewId)).Returns((Review)null); 

            var result = await _reviewService.DeleteReviewAsync(reviewId);

            Assert.Null(result);
        }
    }
}
