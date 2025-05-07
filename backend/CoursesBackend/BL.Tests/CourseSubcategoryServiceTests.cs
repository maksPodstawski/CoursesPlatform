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

namespace BL.Tests
{
    public class CourseSubcategoryServiceTests
    {
        private readonly Mock<ICourseSubcategoryRepository> _mockRepository;
        private readonly CourseSubcategoryService _service;

        public CourseSubcategoryServiceTests()
        {
            _mockRepository = new Mock<ICourseSubcategoryRepository>();
            _service = new CourseSubcategoryService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllCourseSubcategoriesAsync_ReturnsAll()
        {
            var data = new List<CourseSubcategory>
            {
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() },
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockRepository.Setup(r => r.GetCourseSubcategories()).Returns(mockDbSet.Object);

            var result = await _service.GetAllCourseSubcategoriesAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCourseSubcategoryByIdAsync_ExistingId_ReturnsCourseSubcategory()
        {
            var id = Guid.NewGuid();
            var cs = new CourseSubcategory { Id = id, CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() };

            _mockRepository.Setup(r => r.GetCourseSubcategoryByID(id)).Returns(cs);

            var result = await _service.GetCourseSubcategoryByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetCourseSubcategoryByIdAsync_NonExistingId_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetCourseSubcategoryByID(id)).Returns((CourseSubcategory?)null);

            var result = await _service.GetCourseSubcategoryByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCourseIdAsync_ReturnsMatching()
        {
            var courseId = Guid.NewGuid();

            var data = new List<CourseSubcategory>
            {
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = courseId, SubcategoryId = Guid.NewGuid() },
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockRepository.Setup(r => r.GetCourseSubcategories()).Returns(mockDbSet.Object);

            var result = await _service.GetByCourseIdAsync(courseId);

            Assert.Single(result);
            Assert.All(result, cs => Assert.Equal(courseId, cs.CourseId));
        }

        [Fact]
        public async Task GetBySubcategoryIdAsync_ReturnsMatching()
        {
            var subcategoryId = Guid.NewGuid();

            var data = new List<CourseSubcategory>
            {
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = subcategoryId },
                new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockRepository.Setup(r => r.GetCourseSubcategories()).Returns(mockDbSet.Object);

            var result = await _service.GetBySubcategoryIdAsync(subcategoryId);

            Assert.Single(result);
            Assert.All(result, cs => Assert.Equal(subcategoryId, cs.SubcategoryId));
        }

        [Fact]
        public async Task AddCourseSubcategoryAsync_AddsAndReturns()
        {
            var cs = new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() };

            _mockRepository.Setup(r => r.AddCourseSubcategory(It.IsAny<CourseSubcategory>())).Returns(cs);

            var result = await _service.AddCourseSubcategoryAsync(cs);

            Assert.Equal(cs.Id, result.Id);
            _mockRepository.Verify(r => r.AddCourseSubcategory(cs), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseSubcategoryAsync_UpdatesAndReturns()
        {
            var cs = new CourseSubcategory { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() };

            _mockRepository.Setup(r => r.UpdateCourseSubcategory(cs)).Returns(cs);

            var result = await _service.UpdateCourseSubcategoryAsync(cs);

            Assert.NotNull(result);
            Assert.Equal(cs.Id, result?.Id);
            _mockRepository.Verify(r => r.UpdateCourseSubcategory(cs), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseSubcategoryAsync_ExistingId_DeletesAndReturns()
        {
            var id = Guid.NewGuid();
            var cs = new CourseSubcategory { Id = id, CourseId = Guid.NewGuid(), SubcategoryId = Guid.NewGuid() };

            _mockRepository.Setup(r => r.DeleteCourseSubcategory(id)).Returns(cs);

            var result = await _service.DeleteCourseSubcategoryAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result?.Id);
            _mockRepository.Verify(r => r.DeleteCourseSubcategory(id), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseSubcategoryAsync_NonExistingId_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeleteCourseSubcategory(id)).Returns((CourseSubcategory?)null);

            var result = await _service.DeleteCourseSubcategoryAsync(id);

            Assert.Null(result);
            _mockRepository.Verify(r => r.DeleteCourseSubcategory(id), Times.Once);
        }
    }
}

