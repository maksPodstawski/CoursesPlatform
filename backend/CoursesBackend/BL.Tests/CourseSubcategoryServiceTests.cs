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
using MockQueryable;

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


        [Fact]
        public async Task DummyRepository_ShouldNotThrow()
        {
            var service = new CourseSubcategoryService(new DummyCourseSubcategoryRepository());
            var result = await service.GetAllCourseSubcategoriesAsync();
            Assert.Empty(result);
        }

        [Fact]
        public async Task StubRepository_ReturnsSingleItem()
        {
            var expected = new CourseSubcategory { Id = Guid.NewGuid() };
            var service = new CourseSubcategoryService(new StubCourseSubcategoryRepository(expected));
            var result = await service.GetCourseSubcategoryByIdAsync(expected.Id);
            Assert.Equal(expected.Id, result?.Id);
        }

        [Fact]
        public async Task FakeRepository_AddsAndReturns()
        {
            var fakeRepo = new FakeCourseSubcategoryRepository();
            var service = new CourseSubcategoryService(fakeRepo);
            var newCS = new CourseSubcategory { Id = Guid.NewGuid() };
            await service.AddCourseSubcategoryAsync(newCS);
            var result = await service.GetCourseSubcategoryByIdAsync(newCS.Id);
            Assert.Equal(newCS.Id, result?.Id);
        }

        [Fact]
        public async Task SpyRepository_TracksMethodCall()
        {
            var cs = new CourseSubcategory { Id = Guid.NewGuid() };
            var spyRepo = new SpyCourseSubcategoryRepository(cs);
            var service = new CourseSubcategoryService(spyRepo);
            await service.AddCourseSubcategoryAsync(cs);
            Assert.True(spyRepo.WasCalled);
        }


        [Fact]
        public async Task MoqStub_GetCourseSubcategoryById_ReturnsFixedValue()
        {
            var stubValue = new CourseSubcategory { Id = Guid.NewGuid() };
            var repo = new Mock<ICourseSubcategoryRepository>();
            repo.Setup(r => r.GetCourseSubcategoryByID(It.IsAny<Guid>())).Returns(stubValue);
            var service = new CourseSubcategoryService(repo.Object);
            var result = await service.GetCourseSubcategoryByIdAsync(Guid.NewGuid());
            Assert.Equal(stubValue.Id, result?.Id);
        }

        [Fact]
        public async Task MoqSpy_VerifiesMethodCall()
        {
            var cs = new CourseSubcategory { Id = Guid.NewGuid() };
            var repo = new Mock<ICourseSubcategoryRepository>();
            repo.Setup(r => r.AddCourseSubcategory(cs)).Returns(cs);
            var service = new CourseSubcategoryService(repo.Object);
            await service.AddCourseSubcategoryAsync(cs);
            repo.Verify(r => r.AddCourseSubcategory(cs), Times.Once);
        }
    }



    public class DummyCourseSubcategoryRepository : ICourseSubcategoryRepository
    {
        public IQueryable<CourseSubcategory> GetCourseSubcategories()
        {
            var data = new List<CourseSubcategory>().AsQueryable();
            return data.BuildMock();
        }

        public CourseSubcategory? GetCourseSubcategoryByID(Guid id) => null;
        public CourseSubcategory AddCourseSubcategory(CourseSubcategory courseSubcategory) => courseSubcategory;
        public CourseSubcategory? UpdateCourseSubcategory(CourseSubcategory courseSubcategory) => courseSubcategory;
        public CourseSubcategory? DeleteCourseSubcategory(Guid id) => null;
    }

    public class StubCourseSubcategoryRepository : ICourseSubcategoryRepository
    {
        private readonly CourseSubcategory _returnValue;

        public StubCourseSubcategoryRepository(CourseSubcategory returnValue)
        {
            _returnValue = returnValue;
        }

        public CourseSubcategory AddCourseSubcategory(CourseSubcategory cs) => _returnValue;
        public CourseSubcategory? DeleteCourseSubcategory(Guid id) => _returnValue;
        public CourseSubcategory? GetCourseSubcategoryByID(Guid id) => _returnValue;
        public IQueryable<CourseSubcategory> GetCourseSubcategories() => new List<CourseSubcategory> { _returnValue }.AsQueryable();
        public CourseSubcategory UpdateCourseSubcategory(CourseSubcategory cs) => _returnValue;
    }

    public class FakeCourseSubcategoryRepository : ICourseSubcategoryRepository
    {
        private readonly List<CourseSubcategory> _store = new();

        public CourseSubcategory AddCourseSubcategory(CourseSubcategory cs)
        {
            _store.Add(cs);
            return cs;
        }

        public CourseSubcategory? DeleteCourseSubcategory(Guid id)
        {
            var cs = _store.FirstOrDefault(x => x.Id == id);
            if (cs != null) _store.Remove(cs);
            return cs;
        }

        public CourseSubcategory? GetCourseSubcategoryByID(Guid id) => _store.FirstOrDefault(x => x.Id == id);
        public IQueryable<CourseSubcategory> GetCourseSubcategories() => _store.AsQueryable();
        public CourseSubcategory UpdateCourseSubcategory(CourseSubcategory cs)
        {
            DeleteCourseSubcategory(cs.Id);
            return AddCourseSubcategory(cs);
        }
    }

    public class SpyCourseSubcategoryRepository : ICourseSubcategoryRepository
    {
        public bool WasCalled { get; private set; } = false;
        private readonly CourseSubcategory _returnValue;

        public SpyCourseSubcategoryRepository(CourseSubcategory returnValue)
        {
            _returnValue = returnValue;
        }

        public CourseSubcategory AddCourseSubcategory(CourseSubcategory cs)
        {
            WasCalled = true;
            return _returnValue;
        }

        public CourseSubcategory? DeleteCourseSubcategory(Guid id) => _returnValue;
        public CourseSubcategory? GetCourseSubcategoryByID(Guid id) => _returnValue;
        public IQueryable<CourseSubcategory> GetCourseSubcategories() => new List<CourseSubcategory> { _returnValue }.AsQueryable();
        public CourseSubcategory UpdateCourseSubcategory(CourseSubcategory cs) => _returnValue;
    }
}