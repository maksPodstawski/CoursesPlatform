using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using MockQueryable.Moq;
using Xunit;
using BL.Services;

namespace BL.Tests
{
    public class SubcategoryServiceTests
    {
        private readonly Mock<ISubcategoryRepository> _mockSubcategoryRepository;
        private readonly SubcategoryService _subcategoryService;

        public SubcategoryServiceTests()
        {
            _mockSubcategoryRepository = new Mock<ISubcategoryRepository>();
            _subcategoryService = new SubcategoryService(_mockSubcategoryRepository.Object);
        }

        [Fact]
        public async Task GetAllSubcategoriesAsync_ReturnsAllSubcategories()
        {
            var subcategories = new List<Subcategory>
            {
                new Subcategory { Id = Guid.NewGuid(), Name = "CPUs", CategoryId = Guid.NewGuid() },
                new Subcategory { Id = Guid.NewGuid(), Name = "GPUs", CategoryId = Guid.NewGuid() }
            };

            var mockDbSet = subcategories.AsQueryable().BuildMockDbSet();
            _mockSubcategoryRepository.Setup(r => r.GetSubcategories()).Returns(mockDbSet.Object);

            var result = await _subcategoryService.GetAllSubcategoriesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "CPUs");
            Assert.Contains(result, s => s.Name == "GPUs");
        }

        [Fact]
        public async Task GetSubcategoryByIdAsync_ExistingId_ReturnsSubcategory()
        {
            var subcategoryId = Guid.NewGuid();
            var subcategory = new Subcategory { Id = subcategoryId, Name = "Memory", CategoryId = Guid.NewGuid() };

            _mockSubcategoryRepository.Setup(r => r.GetSubcategoryByID(subcategoryId)).Returns(subcategory);

            var result = await _subcategoryService.GetSubcategoryByIdAsync(subcategoryId);

            Assert.NotNull(result);
            Assert.Equal(subcategoryId, result.Id);
            Assert.Equal("Memory", result.Name);
        }

        [Fact]
        public async Task GetSubcategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            var subcategoryId = Guid.NewGuid();
            _mockSubcategoryRepository.Setup(r => r.GetSubcategoryByID(subcategoryId)).Returns((Subcategory?)null);

            var result = await _subcategoryService.GetSubcategoryByIdAsync(subcategoryId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetSubcategoriesByCategoryIdAsync_ReturnsMatchingSubcategories()
        {
            var categoryId = Guid.NewGuid();
            var subcategories = new List<Subcategory>
            {
                new Subcategory { Id = Guid.NewGuid(), Name = "RAM", CategoryId = categoryId },
                new Subcategory { Id = Guid.NewGuid(), Name = "SSD", CategoryId = categoryId },
                new Subcategory { Id = Guid.NewGuid(), Name = "Motherboard", CategoryId = Guid.NewGuid() }
            };

            var mockDbSet = subcategories.AsQueryable().BuildMockDbSet();
            _mockSubcategoryRepository.Setup(r => r.GetSubcategories()).Returns(mockDbSet.Object);

            var result = await _subcategoryService.GetSubcategoriesByCategoryIdAsync(categoryId);

            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal(categoryId, s.CategoryId));
        }

        [Fact]
        public async Task AddSubcategoryAsync_AddsAndReturnsSubcategory()
        {
            var inputSubcategory = new Subcategory { Name = "Cooling", CategoryId = Guid.NewGuid() };

            _mockSubcategoryRepository.Setup(r => r.AddSubcategory(It.IsAny<Subcategory>()))
                .Returns<Subcategory>(s => s);

            var result = await _subcategoryService.AddSubcategoryAsync(inputSubcategory);

            Assert.Equal(inputSubcategory.Name, result.Name);
            Assert.Equal(inputSubcategory.CategoryId, result.CategoryId);
            Assert.NotEqual(Guid.Empty, result.Id);
            _mockSubcategoryRepository.Verify(r => r.AddSubcategory(It.Is<Subcategory>(s => s.Name == "Cooling")), Times.Once);
        }

        [Fact]
        public async Task UpdateSubcategoryAsync_ExistingSubcategory_UpdatesAndReturns()
        {
            var subcategory = new Subcategory { Id = Guid.NewGuid(), Name = "Old Name", CategoryId = Guid.NewGuid() };
            _mockSubcategoryRepository.Setup(r => r.UpdateSubcategory(subcategory)).Returns(subcategory);

            var result = await _subcategoryService.UpdateSubcategoryAsync(subcategory);

            Assert.NotNull(result);
            Assert.Equal(subcategory.Id, result?.Id);
            _mockSubcategoryRepository.Verify(r => r.UpdateSubcategory(subcategory), Times.Once);
        }

        [Fact]
        public async Task DeleteSubcategoryAsync_ExistingId_DeletesAndReturns()
        {
            var subcategoryId = Guid.NewGuid();
            var subcategory = new Subcategory { Id = subcategoryId, Name = "To Delete", CategoryId = Guid.NewGuid() };

            _mockSubcategoryRepository.Setup(r => r.DeleteSubcategory(subcategoryId)).Returns(subcategory);

            var result = await _subcategoryService.DeleteSubcategoryAsync(subcategoryId);

            Assert.NotNull(result);
            Assert.Equal(subcategoryId, result?.Id);
            _mockSubcategoryRepository.Verify(r => r.DeleteSubcategory(subcategoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteSubcategoryAsync_NonExistingId_ReturnsNull()
        {
            var subcategoryId = Guid.NewGuid();
            _mockSubcategoryRepository.Setup(r => r.DeleteSubcategory(subcategoryId)).Returns((Subcategory?)null);

            var result = await _subcategoryService.DeleteSubcategoryAsync(subcategoryId);

            Assert.Null(result);
            _mockSubcategoryRepository.Verify(r => r.DeleteSubcategory(subcategoryId), Times.Once);
        }
    }
}
