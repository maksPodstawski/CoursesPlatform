using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Xunit;

namespace DAL.Tests
{
    public class CourseSubcategoryRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetCourseSubcategories_ReturnsAll()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                context.CourseSubcategories.Add(new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() });
                context.CourseSubcategories.Add(new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.GetCourseSubcategories().ToList();

                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void GetCourseSubcategoryById_Existing_ReturnsSubcategory()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var subcategoryId = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.CourseSubcategories.Add(new CourseSubcategory { Id = id, CourseId = courseId, SubcategoryId = subcategoryId });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.GetCourseSubcategoryByID(id);

                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
                Assert.Equal(courseId, result.CourseId);
                Assert.Equal(subcategoryId, result.SubcategoryId);
            }
        }

        [Fact]
        public void GetCourseSubcategoryById_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.GetCourseSubcategoryByID(Guid.NewGuid());

                Assert.Null(result);
            }
        }

        [Fact]
        public void AddCourseSubcategory_AddsSuccessfully()
        {
            var options = CreateNewContextOptions();
            var entity = new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.AddCourseSubcategory(entity);

                Assert.NotNull(result);
                Assert.Equal(entity.Id, result.Id);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Single(context.CourseSubcategories);
                Assert.Equal(entity.Id, context.CourseSubcategories.Single().Id);
            }
        }

        [Fact]
        public void UpdateCourseSubcategory_Existing_UpdatesSuccessfully()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();
            var oldCourseId = Guid.NewGuid();
            var newCourseId = Guid.NewGuid();
            var oldSubId = Guid.NewGuid();
            var newSubId = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.CourseSubcategories.Add(new CourseSubcategory { Id = id, CourseId = oldCourseId, SubcategoryId = oldSubId });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var updated = new CourseSubcategory { Id = id, CourseId = newCourseId, SubcategoryId = newSubId };
                var result = repo.UpdateCourseSubcategory(updated);

                Assert.NotNull(result);
                Assert.Equal(newCourseId, result.CourseId);
                Assert.Equal(newSubId, result.SubcategoryId);
            }
        }

        [Fact]
        public void UpdateCourseSubcategory_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.UpdateCourseSubcategory(new CourseSubcategory
                {
                    Id = id,
                    CourseId = Guid.NewGuid(),
                    SubcategoryId = Guid.NewGuid()
                });

                Assert.Null(result);
            }
        }

        [Fact]
        public void DeleteCourseSubcategory_Existing_DeletesSuccessfully()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.CourseSubcategories.Add(new CourseSubcategory { Id = id, CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.DeleteCourseSubcategory(id);

                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.CourseSubcategories);
            }
        }

        [Fact]
        public void DeleteCourseSubcategory_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CourseSubcategoryRepository(context);
                var result = repo.DeleteCourseSubcategory(Guid.NewGuid());

                Assert.Null(result);
            }
        }
    }
}
