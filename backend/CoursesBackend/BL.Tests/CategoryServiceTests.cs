using IDAL;
using Model;
using Moq;
using MockQueryable.Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL;
using BL.Services;

namespace BL.Tests
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockRepo;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _service = new CategoryService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Programming" },
                new Category { Id = Guid.NewGuid(), Name = "Design" }
            };
            var mockDbSet = categories.AsQueryable().BuildMockDbSet();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet.Object);

            var result = await _service.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Programming");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ExistingId_ReturnsCategory()
        {
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "AI" };
            _mockRepo.Setup(r => r.GetCategoryById(categoryId)).Returns(category);

            var result = await _service.GetCategoryByIdAsync(categoryId);

            Assert.NotNull(result);
            Assert.Equal("AI", result!.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            _mockRepo.Setup(r => r.GetCategoryById(It.IsAny<Guid>())).Returns((Category)null!);

            var result = await _service.GetCategoryByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ExistingName_ReturnsCategory()
        {
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Backend" },
                new Category { Id = Guid.NewGuid(), Name = "Frontend" }
            };
            var mockDbSet = categories.AsQueryable().BuildMockDbSet();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet.Object);

            var result = await _service.GetCategoryByNameAsync("Frontend");

            Assert.NotNull(result);
            Assert.Equal("Frontend", result!.Name);
        }

        [Fact]
        public async Task GetCategoryByNameAsync_NonExistingName_ReturnsNull()
        {
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Backend" },
                new Category { Id = Guid.NewGuid(), Name = "Frontend" }
            };
            var mockDbSet = categories.AsQueryable().BuildMockDbSet();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet.Object);
            var result = await _service.GetCategoryByNameAsync("NonExisting");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryIdAsync_CategoryWithSubcategories_ReturnsSubcategories()
        {
            var subcategories = new List<Subcategory>
            {
                new Subcategory { Id = Guid.NewGuid(), Name = "React" },
                new Subcategory { Id = Guid.NewGuid(), Name = "Vue" }
            };

            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Frontend", Subcategories = subcategories };

            _mockRepo.Setup(r => r.GetCategoryById(categoryId)).Returns(category);

            var result = await _service.GetSubcategoriesByCategoryIdAsync(categoryId);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryIdAsync_CategoryNotFound_ReturnsEmpty()
        {
            _mockRepo.Setup(r => r.GetCategoryById(It.IsAny<Guid>())).Returns((Category)null!);

            var result = await _service.GetSubcategoriesByCategoryIdAsync(Guid.NewGuid());

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddCategoryAsync_ReturnsAddedCategory()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "DevOps" };
            _mockRepo.Setup(r => r.AddCategory(category)).Returns(category);

            var result = await _service.AddCategoryAsync(category);

            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal("DevOps", result.Name);
            _mockRepo.Verify(r => r.AddCategory(category), Times.Once);
        }

        [Fact]
        public async Task AddCategoryAsync_NullCategory_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddCategoryAsync(null));
            _mockRepo.Verify(r => r.AddCategory(It.IsAny<Category>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task AddCategoryAsync_InvalidName_ThrowsArgumentException(string name)
        {
            var category = new Category { Id = Guid.NewGuid(), Name = name };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddCategoryAsync(category));
            _mockRepo.Verify(r => r.AddCategory(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_RepositoryReturnsNull_ThrowsInvalidOperationException()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };
            _mockRepo.Setup(r => r.AddCategory(category)).Returns((Category)null!);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddCategoryAsync(category));
            _mockRepo.Verify(r => r.AddCategory(category), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsUpdatedCategory()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Mobile" };
            _mockRepo.Setup(r => r.UpdateCategory(category)).Returns(category);

            var result = await _service.UpdateCategoryAsync(category);

            Assert.Equal("Mobile", result!.Name);
            _mockRepo.Verify(r => r.UpdateCategory(category), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_NonExistingCategory_ReturnsNull()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "NonExistent" };
            _mockRepo.Setup(r => r.UpdateCategory(category)).Returns((Category)null!);

            var result = await _service.UpdateCategoryAsync(category);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_NullCategory_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateCategoryAsync(null!));
            _mockRepo.Verify(r => r.UpdateCategory(It.IsAny<Category>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateCategoryAsync_InvalidName_ThrowsArgumentException(string name)
        {
            var category = new Category { Id = Guid.NewGuid(), Name = name };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateCategoryAsync(category));
            _mockRepo.Verify(r => r.UpdateCategory(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsDeletedCategory()
        {
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Game Dev" };
            _mockRepo.Setup(r => r.DeleteCategory(categoryId)).Returns(category);

            var result = await _service.DeleteCategoryAsync(categoryId);

            Assert.Equal("Game Dev", result!.Name);
            _mockRepo.Verify(r => r.DeleteCategory(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_NonExistingCategory_ReturnsNull()
        {
            var categoryId = Guid.NewGuid();
            _mockRepo.Setup(r => r.DeleteCategory(categoryId)).Returns((Category)null!);

            var result = await _service.DeleteCategoryAsync(categoryId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_EmptyGuid_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteCategoryAsync(Guid.Empty));
            _mockRepo.Verify(r => r.DeleteCategory(It.IsAny<Guid>()), Times.Never);
        }
    }
}
