using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class PurchasedCoursesRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private PurchasedCourses CreateSamplePurchasedCourse(Guid? id = null)
        {
            return new PurchasedCourses
            {
                Id = id ?? Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                PurchasedAt = DateTime.UtcNow,
                PurchasedPrice = 49.99m,
                ExpirationDate = DateTime.UtcNow.AddMonths(6),
                IsActive = true
            };
        }

        [Fact]
        public void GetPurchasedCourseByID_WhenNotExists_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new PurchasedCoursesRepository(context);
                var result = repo.GetPurchasedCourseByID(Guid.NewGuid());

                Assert.Null(result);
            }
        }

        [Fact]
        public void GetPurchasedCourses_ReturnsAll()
        {
            var options = CreateNewContextOptions();

            var courses = new List<PurchasedCourses>
            {
                CreateSamplePurchasedCourse(),
                CreateSamplePurchasedCourse()
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.PurchasedCourses.AddRange(courses);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new PurchasedCoursesRepository(context);
                var result = repo.GetPurchasedCourses().ToList();

                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void AddPurchasedCourse_SavesToDatabase()
        {
            var options = CreateNewContextOptions();
            var course = CreateSamplePurchasedCourse();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new PurchasedCoursesRepository(context);
                repo.AddPurchasedCourse(course);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var saved = context.PurchasedCourses.Single();
                Assert.Equal(course.Id, saved.Id);
                Assert.Equal(course.PurchasedPrice, saved.PurchasedPrice);
            }
        }

        [Fact]
        public void UpdatePurchasedCourse_WhenExists_UpdatesAndSaves()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();
            var original = CreateSamplePurchasedCourse(id);

            using (var context = new CoursesPlatformContext(options))
            {
                context.PurchasedCourses.Add(original);
                context.SaveChanges();
            }

            var updated = new PurchasedCourses
            {
                Id = id,
                UserId = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                PurchasedAt = DateTime.UtcNow.AddDays(-1),
                PurchasedPrice = 99.99m,
                ExpirationDate = DateTime.UtcNow.AddMonths(12),
                IsActive = false
            };

            var contextSpy = new DbContextSpy(options);
            var repo = new PurchasedCoursesRepository(contextSpy);
            var result = repo.UpdatePurchasedCourse(updated);

            Assert.NotNull(result);
            Assert.Equal(99.99m, result?.PurchasedPrice);
            Assert.Equal(1, contextSpy.SaveChangesCallCount);
        }

        [Fact]
        public void UpdatePurchasedCourse_WhenNotExists_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            var course = CreateSamplePurchasedCourse();

            var contextSpy = new DbContextSpy(options);
            var repo = new PurchasedCoursesRepository(contextSpy);
            var result = repo.UpdatePurchasedCourse(course);

            Assert.Null(result);
            Assert.Equal(0, contextSpy.SaveChangesCallCount);
        }

        [Fact]
        public void DeletePurchasedCourse_WhenExists_RemovesAndReturnsCourse()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();
            var course = CreateSamplePurchasedCourse(id);

            using (var context = new CoursesPlatformContext(options))
            {
                context.PurchasedCourses.Add(course);
                context.SaveChanges();
            }

            var contextSpy = new DbContextSpy(options);
            var repo = new PurchasedCoursesRepository(contextSpy);
            var deleted = repo.DeletePurchasedCourse(id);

            Assert.NotNull(deleted);
            Assert.Equal(id, deleted?.Id);
            Assert.Equal(1, contextSpy.SaveChangesCallCount);

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.PurchasedCourses);
            }
        }

        [Fact]
        public void DeletePurchasedCourse_WhenNotExists_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            var contextSpy = new DbContextSpy(options);
            var repo = new PurchasedCoursesRepository(contextSpy);
            var result = repo.DeletePurchasedCourse(Guid.NewGuid());

            Assert.Null(result);
            Assert.Equal(0, contextSpy.SaveChangesCallCount);
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
