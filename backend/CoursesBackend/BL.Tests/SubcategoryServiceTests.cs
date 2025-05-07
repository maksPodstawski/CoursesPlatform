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
    public class SubcategoryServiceTests
    {
        private readonly SubcategoryService _subcategoryService;
        private readonly Mock<ISubcategoryRepository> _mockRepository;

        public SubcategoryServiceTests()
        {
            _mockRepository = new Mock<ISubcategoryRepository>();
            _subcategoryService = new SubcategoryService(_mockRepository.Object);
        }

        
        [Fact]
        public async Task DummyTest_DoesNotUseDependency()
        {
            var dummyRepo = new DummySubcategoryRepository();
            var service = new SubcategoryService(dummyRepo);

            // Operacja nie używa repozytorium
            var subcategory = new Subcategory { Name = "Dummy", CategoryId = Guid.NewGuid() };
            await Assert.ThrowsAsync<NotImplementedException>(() => service.AddSubcategoryAsync(subcategory));
        }

        
        [Fact]
        public async Task StubTest_ReturnsPredefinedSubcategory()
        {
            var stubRepo = new StubSubcategoryRepository();
            var service = new SubcategoryService(stubRepo);

            var result = await service.GetSubcategoryByIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.Equal("StubResult", result!.Name);
        }

        
        [Fact]
        public async Task FakeTest_AddAndGetSubcategoryFromMemory()
        {
            var fakeRepo = new FakeSubcategoryRepository();
            var service = new SubcategoryService(fakeRepo);

            var input = new Subcategory { Id = Guid.NewGuid(), Name = "FakeSub", CategoryId = Guid.NewGuid() };
            await service.AddSubcategoryAsync(input);

            var result = await service.GetSubcategoryByIdAsync(input.Id);
            Assert.Equal("FakeSub", result!.Name);
        }

       
        [Fact]
        public async Task MockTest_VerifyAddCalled()
        {
            var subcategory = new Subcategory { Name = "Mocked", CategoryId = Guid.NewGuid() };
            _mockRepository.Setup(r => r.AddSubcategory(It.IsAny<Subcategory>())).Returns(subcategory);

            var result = await _subcategoryService.AddSubcategoryAsync(subcategory);

            Assert.Equal("Mocked", result.Name);
            _mockRepository.Verify(r => r.AddSubcategory(It.Is<Subcategory>(s => s.Name == "Mocked")), Times.Once);
        }

        
        [Fact]
        public async Task SpyTest_TracksInteraction()
        {
            var spyRepo = new SpySubcategoryRepository();
            var service = new SubcategoryService(spyRepo);

            var subcategory = new Subcategory { Id = Guid.NewGuid(), Name = "SpyTest", CategoryId = Guid.NewGuid() };
            await service.UpdateSubcategoryAsync(subcategory);

            Assert.True(spyRepo.WasUpdateCalled);
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
            _mockRepository.Setup(r => r.GetSubcategories()).Returns(mockDbSet.Object);

            var result = await _subcategoryService.GetAllSubcategoriesAsync();

            Assert.Equal(2, result.Count);
        }

       
    }

    
    public class DummySubcategoryRepository : ISubcategoryRepository
    {
        public Subcategory AddSubcategory(Subcategory subcategory) => throw new NotImplementedException();
        public Subcategory DeleteSubcategory(Guid id) => throw new NotImplementedException();
        public IQueryable<Subcategory> GetSubcategories() => throw new NotImplementedException();
        public Subcategory GetSubcategoryByID(Guid id) => throw new NotImplementedException();
        public Subcategory UpdateSubcategory(Subcategory subcategory) => throw new NotImplementedException();
    }

    public class StubSubcategoryRepository : ISubcategoryRepository
    {
        public Subcategory AddSubcategory(Subcategory subcategory) => throw new NotImplementedException();
        public Subcategory DeleteSubcategory(Guid id) => throw new NotImplementedException();
        public IQueryable<Subcategory> GetSubcategories() => throw new NotImplementedException();
        public Subcategory GetSubcategoryByID(Guid id) => new Subcategory { Id = id, Name = "StubResult", CategoryId = Guid.NewGuid() };
        public Subcategory UpdateSubcategory(Subcategory subcategory) => throw new NotImplementedException();
    }

    public class FakeSubcategoryRepository : ISubcategoryRepository
    {
        private readonly List<Subcategory> _subcategories = new();

        public Subcategory AddSubcategory(Subcategory subcategory)
        {
            subcategory.Id = subcategory.Id == Guid.Empty ? Guid.NewGuid() : subcategory.Id;
            _subcategories.Add(subcategory);
            return subcategory;
        }

        public Subcategory DeleteSubcategory(Guid id)
        {
            var item = _subcategories.FirstOrDefault(s => s.Id == id);
            if (item != null) _subcategories.Remove(item);
            return item!;
        }

        public IQueryable<Subcategory> GetSubcategories() => _subcategories.AsQueryable();

        public Subcategory GetSubcategoryByID(Guid id) => _subcategories.FirstOrDefault(s => s.Id == id)!;

        public Subcategory UpdateSubcategory(Subcategory subcategory)
        {
            var existing = _subcategories.FirstOrDefault(s => s.Id == subcategory.Id);
            if (existing != null)
            {
                existing.Name = subcategory.Name;
                existing.CategoryId = subcategory.CategoryId;
            }
            return existing!;
        }
    }

    public class SpySubcategoryRepository : ISubcategoryRepository
    {
        public bool WasUpdateCalled { get; private set; }

        public Subcategory AddSubcategory(Subcategory subcategory) => throw new NotImplementedException();
        public Subcategory DeleteSubcategory(Guid id) => throw new NotImplementedException();
        public IQueryable<Subcategory> GetSubcategories() => throw new NotImplementedException();
        public Subcategory GetSubcategoryByID(Guid id) => throw new NotImplementedException();

        public Subcategory UpdateSubcategory(Subcategory subcategory)
        {
            WasUpdateCalled = true;
            return subcategory;
        }
    }
}
