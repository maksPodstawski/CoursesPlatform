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

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();

            var users = new List<User>
            {
                new User { Id = userId1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new User { Id = userId2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };

            var courses = new List<Course>
            {
                new Course { Id = courseId1, Name = "Course 1", Description = "Description 1" },
                new Course { Id = courseId2, Name = "Course 2", Description = "Description 2" }
            };

            var testReviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 5, Comment = "Great!", UserId = userId1, CourseId = courseId1, CreatedAt = DateTime.UtcNow },
                new Review { Id = Guid.NewGuid(), Rating = 4, Comment = "Good", UserId = userId2, CourseId = courseId2, CreatedAt = DateTime.UtcNow }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.AddRange(users);
                context.Courses.AddRange(courses);
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

            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Courses.Add(course);
                context.SaveChanges();
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                Rating = 5,
                Comment = "Excellent course",
                UserId = userId,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ReviewRepository(context);
                var result = repo.AddReview(review);

                Assert.NotNull(result);
                Assert.Equal("Excellent course", result.Comment);
                Assert.Equal(5, result.Rating);
                Assert.NotNull(result.User);
                Assert.NotNull(result.Course);
            }
        }

        [Fact]
        public void UpdateReview_WhenExists_UpdatesAndSavesReview()
        {
            var options = CreateNewContextOptions();

            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Courses.Add(course);
                context.SaveChanges();
            }

            var original = new Review
            {
                Id = reviewId,
                Rating = 3,
                Comment = "Okay",
                UserId = userId,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
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
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ReviewRepository(context);
                var result = repo.UpdateReview(updated);

                Assert.NotNull(result);
                Assert.Equal("Better now", result.Comment);
                Assert.Equal(4, result.Rating);
                Assert.NotNull(result.User);
                Assert.NotNull(result.Course);
            }
        }

        [Fact]
        public void DeleteReview_WhenExists_RemovesReview()
        {
            var options = CreateNewContextOptions();

            var reviewId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var course = new Course { Id = courseId, Name = "Test Course", Description = "Test Description" };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Courses.Add(course);
                context.SaveChanges();
            }

            var review = new Review
            {
                Id = reviewId,
                Rating = 2,
                Comment = "Not good",
                UserId = userId,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Reviews.Add(review);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ReviewRepository(context);
                var result = repo.DeleteReview(reviewId);

                Assert.NotNull(result);
                Assert.Equal("Not good", result.Comment);
                Assert.NotNull(result.User);
                Assert.NotNull(result.Course);

                Assert.Empty(context.Reviews);
            }
        }
    }
}
