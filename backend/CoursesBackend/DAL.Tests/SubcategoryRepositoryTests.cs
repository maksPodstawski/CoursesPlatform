using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Xunit;

namespace DAL.Tests
{
    public class SubcategoryRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetSubcategories_ReturnsAll()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Subcategories.Add(new Subcategory { Id = Guid.NewGuid(), Name = "Sub 1", CategoryId = Guid.NewGuid() });
                context.Subcategories.Add(new Subcategory { Id = Guid.NewGuid(), Name = "Sub 2", CategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.GetSubcategories().ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, s => s.Name == "Sub 1");
                Assert.Contains(result, s => s.Name == "Sub 2");
            }
        }

        [Fact]
        public void GetSubcategoryById_Existing_ReturnsSubcategory()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Subcategories.Add(new Subcategory { Id = id, Name = "Subcategory", CategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.GetSubcategoryByID(id);

                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
                Assert.Equal("Subcategory", result.Name);
            }
        }

        [Fact]
        public void GetSubcategoryById_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.GetSubcategoryByID(Guid.NewGuid());

                Assert.Null(result);
            }
        }

        [Fact]
        public void AddSubcategory_AddsSuccessfully()
        {
            var options = CreateNewContextOptions();
            var subcategory = new Subcategory { Id = Guid.NewGuid(), Name = "New Sub", CategoryId = Guid.NewGuid() };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.AddSubcategory(subcategory);

                Assert.NotNull(result);
                Assert.Equal(subcategory.Id, result.Id);
                Assert.Equal("New Sub", result.Name);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Single(context.Subcategories);
                Assert.Equal("New Sub", context.Subcategories.Single().Name);
            }
        }

        [Fact]
        public void UpdateSubcategory_Existing_UpdatesSuccessfully()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Subcategories.Add(new Subcategory { Id = id, Name = "Old Name", CategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var sub = context.Subcategories.Find(id);
                sub.Name = "Updated Name";

                var result = repo.UpdateSubcategory(sub);

                Assert.NotNull(result);
                Assert.Equal("Updated Name", result.Name);
            }
        }

        [Fact]
        public void UpdateSubcategory_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.UpdateSubcategory(new Subcategory { Id = Guid.NewGuid(), Name = "Fake", CategoryId = Guid.NewGuid() });

                Assert.Null(result);
            }
        }

        [Fact]
        public void DeleteSubcategory_Existing_DeletesSuccessfully()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Subcategories.Add(new Subcategory { Id = id, Name = "To delete", CategoryId = Guid.NewGuid() });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.DeleteSubcategory(id);

                Assert.NotNull(result);
                Assert.Equal(id, result.Id);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Subcategories);
            }
        }

        [Fact]
        public void DeleteSubcategory_NonExisting_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new SubcategoryRepository(context);
                var result = repo.DeleteSubcategory(Guid.NewGuid());

                Assert.Null(result);
            }
        }
    }
}

