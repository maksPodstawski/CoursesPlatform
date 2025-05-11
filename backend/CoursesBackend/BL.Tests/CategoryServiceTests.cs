using IDAL;
using Model;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBL;
using MockQueryable;
using Moq;

namespace BL.Tests
{
    public class CategoryServiceTests
    {
        private readonly Moq.Mock<ICategoryRepository> _mockRepo;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockRepo = new Moq.Mock<ICategoryRepository>();
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

            var mockDbSet = categories.AsQueryable().BuildMock();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet);

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

            var mockDbSet = categories.AsQueryable().BuildMock();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet);

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

            var mockDbSet = categories.AsQueryable().BuildMock();
            _mockRepo.Setup(r => r.GetCategories()).Returns(mockDbSet);

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
        [InlineData("   ")]
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

        private class DummyCategoryRepository : ICategoryRepository
        {
            public Category AddCategory(Category category) => throw new NotImplementedException();
            public Category DeleteCategory(Guid id) => throw new NotImplementedException();
            public IQueryable<Category> GetCategories() => throw new NotImplementedException();
            public Category GetCategoryById(Guid id) => throw new NotImplementedException();
            public Category UpdateCategory(Category category) => throw new NotImplementedException();
        }

        [Fact]
        public void Dummy_IsAcceptedByConstructor()
        {
            var service = new CategoryService(new DummyCategoryRepository());
            Assert.NotNull(service);
        }

        private class StubCategoryRepository : ICategoryRepository
        {
            public Category AddCategory(Category category) => throw new NotImplementedException();
            public Category DeleteCategory(Guid id) => throw new NotImplementedException();
            public IQueryable<Category> GetCategories() => throw new NotImplementedException();
            public Category GetCategoryById(Guid id) => new Category { Id = id, Name = "Stubbed" };
            public Category UpdateCategory(Category category) => throw new NotImplementedException();
        }

        [Fact]
        public async Task Stub_ReturnsStubbedCategory()
        {
            var service = new CategoryService(new StubCategoryRepository());
            var result = await service.GetCategoryByIdAsync(Guid.NewGuid());
            Assert.Equal("Stubbed", result!.Name);
        }

        private class FakeCategoryRepository : ICategoryRepository
        {
            private readonly List<Category> _store = new();
            public Category AddCategory(Category category) { _store.Add(category); return category; }
            public Category DeleteCategory(Guid id) => throw new NotImplementedException();
            public IQueryable<Category> GetCategories() => _store.AsQueryable();
            public Category GetCategoryById(Guid id) => _store.FirstOrDefault(c => c.Id == id)!;
            public Category UpdateCategory(Category category) => throw new NotImplementedException();
        }

        [Fact]
        public async Task Fake_AddsAndGetsCategory()
        {
            var repo = new FakeCategoryRepository();
            var service = new CategoryService(repo);
            var cat = new Category { Id = Guid.NewGuid(), Name = "FakeCat" };

            await service.AddCategoryAsync(cat);
            var result = await service.GetCategoryByIdAsync(cat.Id);

            Assert.Equal("FakeCat", result!.Name);
        }

        private class SpyCategoryRepository : ICategoryRepository
        {
            public int AddCallCount { get; private set; } = 0;
            public Category? LastAdded { get; private set; }
            public Category AddCategory(Category category) { AddCallCount++; LastAdded = category; return category; }
            public Category DeleteCategory(Guid id) => throw new NotImplementedException();
            public IQueryable<Category> GetCategories() => throw new NotImplementedException();
            public Category GetCategoryById(Guid id) => throw new NotImplementedException();
            public Category UpdateCategory(Category category) => throw new NotImplementedException();
        }

        [Fact]
        public async Task Spy_TracksAddCall()
        {
            var spy = new SpyCategoryRepository();
            var service = new CategoryService(spy);
            var cat = new Category { Id = Guid.NewGuid(), Name = "SpyCat" };

            await service.AddCategoryAsync(cat);

            Assert.Equal(1, spy.AddCallCount);
            Assert.Equal("SpyCat", spy.LastAdded!.Name);
        }

        private class ManualMockCategoryRepository : ICategoryRepository
        {
            public Func<Guid, Category?> GetByIdFunc = id => null;
            public Category AddCategory(Category category) => throw new NotImplementedException();
            public Category DeleteCategory(Guid id) => throw new NotImplementedException();
            public IQueryable<Category> GetCategories() => throw new NotImplementedException();
            public Category GetCategoryById(Guid id) => GetByIdFunc(id)!;
            public Category UpdateCategory(Category category) => throw new NotImplementedException();
        }

        [Fact]
        public async Task ManualMock_ReturnsConfiguredCategory()
        {
            var mock = new ManualMockCategoryRepository
            {
                GetByIdFunc = id => new Category { Id = id, Name = "ManualMock" }
            };

            var service = new CategoryService(mock);
            var result = await service.GetCategoryByIdAsync(Guid.NewGuid());

            Assert.Equal("ManualMock", result!.Name);
        }
    }
}