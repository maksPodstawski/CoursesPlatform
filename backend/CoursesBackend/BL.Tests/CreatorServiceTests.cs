using BL.Services;
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
    public class CreatorServiceTests
    {
        private readonly Mock<ICreatorRepository> _mockCreatorRepo;
        private readonly Mock<ICourseRepository> _mockCourseRepo;
        private readonly CreatorService _service;

        public CreatorServiceTests()
        {
            _mockCreatorRepo = new Mock<ICreatorRepository>();
            _mockCourseRepo = new Mock<ICourseRepository>();
            _service = new CreatorService(_mockCreatorRepo.Object, _mockCourseRepo.Object);
        }

        [Fact]
        public async Task GetAllCreatorsAsync_ReturnsAllCreators()
        {
            var creators = new List<Creator>
            {
                new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
                new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() }
            };
            var mockDbSet = creators.AsQueryable().BuildMockDbSet();
            _mockCreatorRepo.Setup(r => r.GetCreators()).Returns(mockDbSet.Object);

            var result = await _service.GetAllCreatorsAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCreatorByIdAsync_ExistingId_ReturnsCreator()
        {
            var creatorId = Guid.NewGuid();
            var creator = new Creator { Id = creatorId, UserId = Guid.NewGuid() };
            _mockCreatorRepo.Setup(r => r.GetCreatorByID(creatorId)).Returns(creator);

            var result = await _service.GetCreatorByIdAsync(creatorId);

            Assert.NotNull(result);
            Assert.Equal(creatorId, result!.Id);
        }

        [Fact]
        public async Task GetCreatorByIdAsync_NonExistingId_ReturnsNull()
        {
            _mockCreatorRepo.Setup(r => r.GetCreatorByID(It.IsAny<Guid>())).Returns((Creator)null!);

            var result = await _service.GetCreatorByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task AddCreatorAsync_ValidCreator_ReturnsCreator()
        {
            var creator = new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            _mockCreatorRepo.Setup(r => r.AddCreator(creator)).Returns(creator);

            var result = await _service.AddCreatorAsync(creator);

            Assert.Equal(creator.Id, result.Id);
            _mockCreatorRepo.Verify(r => r.AddCreator(creator), Times.Once);
        }

        [Fact]
        public async Task AddCreatorAsync_NullCreator_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddCreatorAsync(null!));
            _mockCreatorRepo.Verify(r => r.AddCreator(It.IsAny<Creator>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCreatorAsync_ValidId_ReturnsCreator()
        {
            var creatorId = Guid.NewGuid();
            var creator = new Creator { Id = creatorId };
            _mockCreatorRepo.Setup(r => r.DeleteCreator(creatorId)).Returns(creator);

            var result = await _service.DeleteCreatorAsync(creatorId);

            Assert.Equal(creatorId, result!.Id);
            _mockCreatorRepo.Verify(r => r.DeleteCreator(creatorId), Times.Once);
        }

        [Fact]
        public async Task DeleteCreatorAsync_InvalidId_ReturnsNull()
        {
            _mockCreatorRepo.Setup(r => r.DeleteCreator(It.IsAny<Guid>())).Returns((Creator)null!);

            var result = await _service.DeleteCreatorAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCoursesByCreatorAsync_CreatorWithCourses_ReturnsCourses()
        {
            var creatorId = Guid.NewGuid();
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1" },
                new Course { Id = Guid.NewGuid(), Name = "Course 2" }
            };

            var creators = new List<Creator>
            {
                new Creator { Id = Guid.NewGuid(), UserId = creatorId, Courses = courses }
            };

            var mockDbSet = creators.AsQueryable().BuildMockDbSet();
            _mockCreatorRepo.Setup(r => r.GetCreators()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByCreatorAsync(creatorId);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Course 1");
            Assert.Contains(result, c => c.Name == "Course 2");
        }

        [Fact]
        public async Task GetCoursesByCreatorAsync_NoCourses_ReturnsEmpty()
        {
            var userId = Guid.NewGuid();
            var creators = new List<Creator>
            {
                new Creator { Id = Guid.NewGuid(), UserId = userId, Courses = new List<Course>() }
            };

            var mockDbSet = creators.AsQueryable().BuildMockDbSet();
            _mockCreatorRepo.Setup(r => r.GetCreators()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByCreatorAsync(userId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task IsUserCreatorOfCourseAsync_MatchFound_ReturnsTrue()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var creator = new Creator
            {
                UserId = userId,
                Courses = new List<Course> { new() { Id = courseId } }
            };

            var mockCreators = new List<Creator> { creator }.AsQueryable().BuildMockDbSet();
            _mockCreatorRepo.Setup(r => r.GetCreators()).Returns(mockCreators.Object);

            var result = await _service.IsUserCreatorOfCourseAsync(userId, courseId);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUserCreatorOfCourseAsync_NoMatch_ReturnsFalse()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var creator = new Creator
            {
                UserId = userId,
                Courses = new List<Course> { new() { Id = Guid.NewGuid() } }
            };

            var mockCreators = new List<Creator> { creator }.AsQueryable().BuildMockDbSet();
            _mockCreatorRepo.Setup(r => r.GetCreators()).Returns(mockCreators.Object);

            var result = await _service.IsUserCreatorOfCourseAsync(userId, courseId);

            Assert.False(result);
        }

        [Fact]
        public async Task AddCreatorFromUserAsync_ValidData_ReturnsCreator()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var course = new Course { Id = courseId };
            var expectedCreator = new Creator { UserId = userId, Courses = new List<Course> { course } };

            _mockCourseRepo.Setup(r => r.GetCourseById(courseId)).Returns(course);
            _mockCreatorRepo.Setup(r => r.AddCreator(It.Is<Creator>(c => c.UserId == userId))).Returns(expectedCreator);

            var result = await _service.AddCreatorFromUserAsync(userId, courseId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Contains(result.Courses, c => c.Id == courseId);
        }

        [Fact]
        public async Task AddCreatorFromUserAsync_CourseNotFound_ThrowsArgumentNullExceptions()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            _mockCourseRepo.Setup(r => r.GetCourseById(courseId)).Returns((Course)null!);
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddCreatorFromUserAsync(userId, courseId));
        }
    }
}
