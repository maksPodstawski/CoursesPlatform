using BL.Services;
using IDAL;
using MockQueryable.Moq;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        [Fact]
        public async Task StubRepository_ReturnsPredefinedReviewById()
        {
            var reviewId = Guid.NewGuid();
            var service = new ReviewService(new StubReviewRepository(reviewId));
            var result = await service.GetReviewByIdAsync(reviewId);
            Assert.NotNull(result);
            Assert.Equal(reviewId, result.Id);
        }
        [Fact]
        public async Task MockRepository_VerifiesDeleteCall()
        {
            var reviewId = Guid.NewGuid();
            var repository = new MockReviewRepository(reviewId);
            var service = new ReviewService(repository);
            await service.DeleteReviewAsync(reviewId);
            Assert.True(repository.DeleteCalled);
        }

        [Fact]
        public async Task SpyRepository_TracksAddReviewCall()
        {
            var repository = new SpyReviewRepository();
            var service = new ReviewService(repository);
            var review = new Review { Rating = 3, Comment = "Spy", CourseId = Guid.NewGuid(), UserId = Guid.NewGuid() };
            await service.AddReviewAsync(review);
            Assert.True(repository.AddCalled);
            Assert.Equal("Spy", repository.LastAdded?.Comment);
        }
    }

    public class DummyReviewRepository : IReviewRepository
    {
        public Review AddReview(Review review) => review;
        public Review? DeleteReview(Guid reviewId) => null;
        public Review? GetReviewById(Guid reviewId) => null;
        public IQueryable<Review> GetReviews() => new List<Review>().AsQueryable();
        public Review? UpdateReview(Review review) => review;
    }

    public class StubReviewRepository : IReviewRepository
    {
        private readonly Guid _reviewId;

        public StubReviewRepository(Guid reviewId)
        {
            _reviewId = reviewId;
        }

        public Review AddReview(Review review) => review;
        public Review? DeleteReview(Guid reviewId) => null;
        public Review? GetReviewById(Guid reviewId)
        {
            if (reviewId == _reviewId)
                return new Review { Id = reviewId, Comment = "Stub" };
            return null;
        }

        public IQueryable<Review> GetReviews() => new List<Review>().AsQueryable();
        public Review? UpdateReview(Review review) => review;
    }

    public class FakeReviewRepository : IReviewRepository
    {
        private readonly List<Review> _reviews = new();

        public Review AddReview(Review review)
        {
            review.Id = Guid.NewGuid();
            _reviews.Add(review);
            return review;
        }

        public Review? DeleteReview(Guid reviewId)
        {
            var review = _reviews.FirstOrDefault(r => r.Id == reviewId);
            if (review != null) _reviews.Remove(review);
            return review;
        }

        public Review? GetReviewById(Guid reviewId) => _reviews.FirstOrDefault(r => r.Id == reviewId);

        public IQueryable<Review> GetReviews() => _reviews.AsQueryable();

        public Review? UpdateReview(Review review)
        {
            var existing = _reviews.FirstOrDefault(r => r.Id == review.Id);
            if (existing != null)
            {
                existing.Comment = review.Comment;
                existing.Rating = review.Rating;
            }
            return existing;
        }
    }

    public class MockReviewRepository : IReviewRepository
    {
        private readonly Guid _expectedId;
        public bool DeleteCalled { get; private set; }

        public MockReviewRepository(Guid expectedId)
        {
            _expectedId = expectedId;
        }

        public Review AddReview(Review review) => review;
        public Review? DeleteReview(Guid reviewId)
        {
            DeleteCalled = reviewId == _expectedId;
            return DeleteCalled ? new Review { Id = reviewId, Comment = "Deleted" } : null;
        }

        public Review? GetReviewById(Guid reviewId) => null;
        public IQueryable<Review> GetReviews() => new List<Review>().AsQueryable();
        public Review? UpdateReview(Review review) => review;
    }

    public class SpyReviewRepository : IReviewRepository
    {
        public bool AddCalled { get; private set; }
        public Review? LastAdded { get; private set; }

        public Review AddReview(Review review)
        {
            AddCalled = true;
            LastAdded = review;
            review.Id = Guid.NewGuid();
            return review;
        }

        public Review? DeleteReview(Guid reviewId) => null;
        public Review? GetReviewById(Guid reviewId) => null;
        public IQueryable<Review> GetReviews() => new List<Review>().AsQueryable();
        public Review? UpdateReview(Review review) => review;
    }
}
