using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class CategoryRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetCategories_ShouldReturnAllCategories()
        {
            // Arrange
            var options = GetInMemoryDbOptions();

            using var context = new CoursesPlatformContext(options);

            context.Categories.AddRange(new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category 1" },
                new Category { Id = Guid.NewGuid(), Name = "Category 2" }
            });
            context.SaveChanges();

            var repository = new CategoryRepository(context);

            // Act
            var categories = repository.GetCategories().ToList();

            // Assert
            Assert.Equal(2, categories.Count);
        }

        [Fact]
        public void GetCategoryById_ShouldReturnCorrectCategory()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            var categoryId = Guid.NewGuid();

            using var context = new CoursesPlatformContext(options);
            context.Categories.Add(new Category { Id = categoryId, Name = "Category 1" });
            context.SaveChanges();

            var repository = new CategoryRepository(context);

            // Act
            var category = repository.GetCategoryById(categoryId);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Category 1", category.Name);
        }

        [Fact]
        public void GetCategoryById_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange Fake
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CategoryRepository(context);

            // Act
            var category = repository.GetCategoryById(Guid.NewGuid());

            // Assert
            Assert.Null(category);
        }

        [Fact]
        public void AddCategory_ShouldAddCategoryToDatabase()
        {
            // Arrange 
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CategoryRepository(context);
            var newCategory = new Category { Id = Guid.NewGuid(), Name = "New Category" };

            // Act
            var addedCategory = repository.AddCategory(newCategory);

            // Assert
            Assert.NotNull(addedCategory);
            Assert.Equal("New Category", addedCategory.Name);
            Assert.Single(context.Categories);
        }

        [Fact]
        public void UpdateCategory_ShouldUpdateExistingCategory()
        {
            // Arrange 
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var categoryId = Guid.NewGuid();
            context.Categories.Add(new Category { Id = categoryId, Name = "Old Name" });
            context.SaveChanges();

            var repository = new CategoryRepository(context);
            var updatedCategory = new Category { Id = categoryId, Name = "Updated Name" };

            // Act
            var result = repository.UpdateCategory(updatedCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
        }

        [Fact]
        public void DeleteCategory_ShouldRemoveCategoryFromDatabase()
        {
            // Arrange 
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var categoryId = Guid.NewGuid();
            context.Categories.Add(new Category { Id = categoryId, Name = "Category to Delete" });
            context.SaveChanges();

            var repository = new CategoryRepository(context);

            // Act
            var deletedCategory = repository.DeleteCategory(categoryId);

            // Assert
            Assert.NotNull(deletedCategory);
            Assert.Equal("Category to Delete", deletedCategory.Name);
            Assert.Empty(context.Categories);
        }
    }
}
