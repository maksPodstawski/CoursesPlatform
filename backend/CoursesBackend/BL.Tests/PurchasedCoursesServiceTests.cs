using IDAL;
using MockQueryable.Moq;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Tests
{
    public class PurchasedCoursesServiceTests
    {
        private readonly Mock<IPurchasedCoursesRepository> _mockPurchasedCoursesRepository;
        private readonly PurchasedCoursesService _purchasedCoursesservice;

        public PurchasedCoursesServiceTests()
        {
            _mockPurchasedCoursesRepository = new Mock<IPurchasedCoursesRepository>();
            _purchasedCoursesservice = new PurchasedCoursesService(_mockPurchasedCoursesRepository.Object);
        }

        [Fact]
        public async Task GetAllPurchasedCoursesAsync_ReturnsAllCourses()
        {
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses(),
                new PurchasedCourses(),
                new PurchasedCourses()
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetAllPurchasedCoursesAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetPurchasedCourseByIdAsync_ExistingId_ReturnsCourse()
        {
            var target = new PurchasedCourses { Id = Guid.NewGuid() };

            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourseByID(target.Id)).Returns(target);

            var result = await _purchasedCoursesservice.GetPurchasedCourseByIdAsync(target.Id);

            Assert.NotNull(result);
            Assert.Equal(target.Id, result.Id);
        }

        [Fact]
        public async Task GetPurchasedCourseByIdAsync_NonExistingId_ReturnsNull()
        {
            var id = Guid.NewGuid();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourseByID(id)).Returns((PurchasedCourses?)null);

            var result = await _purchasedCoursesservice.GetPurchasedCourseByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetPurchasedCoursesByUserIdAsync_ReturnsCorrectCourses()
        {
            var userId = Guid.NewGuid();
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { UserId = userId },
                new PurchasedCourses { UserId = Guid.NewGuid() },
                new PurchasedCourses { UserId = userId }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetPurchasedCoursesByUserIdAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, pc => Assert.Equal(userId, pc.UserId));
        }

        [Fact]
        public async Task GetPurchasedCoursesByCourseIdAsync_ReturnsCorrectCourses()
        {
            var courseId = Guid.NewGuid();
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { CourseId = Guid.NewGuid() },
                new PurchasedCourses { CourseId = courseId },
                new PurchasedCourses { CourseId = Guid.NewGuid() }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetPurchasedCoursesByCourseIdAsync(courseId);

            Assert.Single(result);
            Assert.Equal(courseId, result[0].CourseId);
        }

        [Fact]
        public async Task GetActivePurchasedCoursesAsync_ReturnsOnlyActive()
        {
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { IsActive = true },
                new PurchasedCourses { IsActive = false },
                new PurchasedCourses { IsActive = true }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetActivePurchasedCoursesAsync();

            Assert.Equal(2, result.Count);
            Assert.All(result, pc => Assert.True(pc.IsActive));
        }

        [Fact]
        public async Task GetExpiredPurchasedCoursesAsync_ReturnsOnlyExpired()
        {
            var now = DateTime.UtcNow;
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { ExpirationDate = now.AddDays(-1) },
                new PurchasedCourses { ExpirationDate = now.AddDays(5) },
                new PurchasedCourses { ExpirationDate = null }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetExpiredPurchasedCoursesAsync();

            Assert.Single(result);
            Assert.True(result[0].ExpirationDate < now);
        }

        [Fact]
        public async Task GetActiveCoursesByUserSortedAsync_ReturnsSortedActive()
        {
            var userId = Guid.NewGuid();
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { UserId = userId, IsActive = true, PurchasedAt = DateTime.UtcNow.AddDays(-1) },
                new PurchasedCourses { UserId = userId, IsActive = true, PurchasedAt = DateTime.UtcNow },
                new PurchasedCourses { UserId = Guid.NewGuid(), IsActive = true, PurchasedAt = DateTime.UtcNow.AddDays(-2) },
                new PurchasedCourses { UserId = userId, IsActive = false, PurchasedAt = DateTime.UtcNow }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.GetActiveCoursesByUserSortedAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.True(result[0].PurchasedAt >= result[1].PurchasedAt);
        }

        [Fact]
        public async Task HasUserPurchasedCourseAsync_ReturnsTrueIfExists()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { UserId = userId, CourseId = courseId }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.HasUserPurchasedCourseAsync(userId, courseId);

            Assert.True(result);
        }

        [Fact]
        public async Task HasUserPurchasedCourseAsync_ReturnsFalseIfNotExists()
        {
            var data = new List<PurchasedCourses>
            {
                new PurchasedCourses { UserId = Guid.NewGuid(), CourseId = Guid.NewGuid() }
            };

            var mockDbSet = data.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _purchasedCoursesservice.HasUserPurchasedCourseAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task AddPurchasedCourseAsync_AddsAndReturnsCourse()
        {
            var pc = new PurchasedCourses { Id = Guid.NewGuid() };

            _mockPurchasedCoursesRepository.Setup(r => r.AddPurchasedCourse(It.IsAny<PurchasedCourses>())).Returns(pc);

            var result = await _purchasedCoursesservice.AddPurchasedCourseAsync(pc);

            Assert.Equal(pc.Id, result.Id);
            _mockPurchasedCoursesRepository.Verify(r => r.AddPurchasedCourse(pc), Times.Once);
        }

        [Fact]
        public async Task UpdatePurchasedCourseAsync_UpdatesAndReturnsCourse()
        {
            var pc = new PurchasedCourses { Id = Guid.NewGuid(), IsActive = false };

            _mockPurchasedCoursesRepository.Setup(r => r.UpdatePurchasedCourse(It.IsAny<PurchasedCourses>())).Returns(pc);

            var result = await _purchasedCoursesservice.UpdatePurchasedCourseAsync(pc);

            Assert.Equal(pc.Id, result?.Id);
            _mockPurchasedCoursesRepository.Verify(r => r.UpdatePurchasedCourse(pc), Times.Once);
        }

        [Fact]
        public async Task DeletePurchasedCourseAsync_DeletesAndReturnsCourse()
        {
            var id = Guid.NewGuid();
            var pc = new PurchasedCourses { Id = id };

            _mockPurchasedCoursesRepository.Setup(r => r.DeletePurchasedCourse(id)).Returns(pc);

            var result = await _purchasedCoursesservice.DeletePurchasedCourseAsync(id);

            Assert.Equal(id, result?.Id);
            _mockPurchasedCoursesRepository.Verify(r => r.DeletePurchasedCourse(id), Times.Once);
        }
    }
}
