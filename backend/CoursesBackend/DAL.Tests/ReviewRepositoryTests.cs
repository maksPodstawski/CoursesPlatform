using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class ReviewRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetReviewById_WhenReviewDoesNotExist_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            var dummyId = Guid.NewGuid(); 

            using var context = new CoursesPlatformContext(options);
            var repo = new ReviewRepository(context);

            var result = repo.GetReviewById(dummyId);

            Assert.Null(result);
        }

        [Fact]
        public void GetReviews_ReturnsAllReviews()
        {
            var options = CreateNewContextOptions();

            var testReviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 5, Comment = "Great!", UserId = Guid.NewGuid(), CourseId = Guid.NewGuid() },
                new Review { Id = Guid.NewGuid(), Rating = 4, Comment = "Good", UserId = Guid.NewGuid(), CourseId = Guid.NewGuid() }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Reviews.AddRange(testReviews);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ReviewRepository(context);
                var result = repo.GetReviews().ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, r => r.Comment == "Great!");
                Assert.Contains(result, r => r.Rating == 4);
            }
        }

        [Fact]
        public void AddReview_SavesReviewToDatabase()
        {
            var options = CreateNewContextOptions();

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Rating = 5,
                Comment = "Excellent course",
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ReviewRepository(context);
                repo.AddReview(review);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var saved = context.Reviews.Single();
                Assert.Equal("Excellent course", saved.Comment);
                Assert.Equal(5, saved.Rating);
            }
        }

        [Fact]
        public void UpdateReview_WhenExists_UpdatesAndSavesReview()
        {
            var options = CreateNewContextOptions();

            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var original = new Review
            {
                Id = reviewId,
                Rating = 3,
                Comment = "Okay",
                UserId = userId,
                CourseId = courseId
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Reviews.Add(original);
                context.SaveChanges();
            }

            var updated = new Review
            {
                Id = reviewId,
                Rating = 4,
                Comment = "Better now",
                UserId = userId,
                CourseId = courseId
            };

            var spyContext = new DbContextSpy(options);
            var repo = new ReviewRepository(spyContext);
            var result = repo.UpdateReview(updated);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.NotNull(result);
            Assert.Equal("Better now", result.Comment);
            Assert.Equal(4, result.Rating);
        }

        [Fact]
        public void DeleteReview_WhenExists_RemovesReview()
        {
            var options = CreateNewContextOptions();

            var reviewId = Guid.NewGuid();
            var review = new Review
            {
                Id = reviewId,
                Rating = 2,
                Comment = "Not good",
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid()
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Reviews.Add(review);
                context.SaveChanges();
            }

            var spyContext = new DbContextSpy(options);
            var repo = new ReviewRepository(spyContext);
            var result = repo.DeleteReview(reviewId);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.Equal("Not good", result?.Comment);

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Reviews);
            }
        }

        private class DbContextSpy : CoursesPlatformContext
        {
            public int SaveChangesCallCount { get; private set; } = 0;

            public DbContextSpy(DbContextOptions<CoursesPlatformContext> options) : base(options) { }

            public override int SaveChanges()
            {
                SaveChangesCallCount++;
                return base.SaveChanges();
            }
        }
    }
}
