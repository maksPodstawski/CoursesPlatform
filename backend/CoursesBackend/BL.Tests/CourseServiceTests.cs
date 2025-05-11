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
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _mockCourseRepo;
        private readonly CourseService _service;

        public CourseServiceTests()
        {
            _mockCourseRepo = new Mock<ICourseRepository>();
            _service = new CourseService(_mockCourseRepo.Object);
        }

        [Fact]
        public async Task GetAllCoursesAsync_ReturnsAllCourses()
        {
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1" },
                new Course { Id = Guid.NewGuid(), Name = "Course 2" }
            };

            var mockDbSet = courses.AsQueryable().BuildMockDbSet();
            _mockCourseRepo.Setup(r => r.GetCourses()).Returns(mockDbSet.Object);

            var result = await _service.GetAllCoursesAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCourseByIdAsync_ExistingId_ReturnsCourse()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Course 1" };
            _mockCourseRepo.Setup(r => r.GetCourseById(courseId)).Returns(course);

            var result = await _service.GetCourseByIdAsync(courseId);

            Assert.NotNull(result);
            Assert.Equal(courseId, result!.Id);
        }

        [Fact]
        public async Task GetCourseByIdAsync_NonExistingId_ReturnsNull()
        {
            var courseId = Guid.NewGuid();
            _mockCourseRepo.Setup(r => r.GetCourseById(courseId)).Returns((Course)null);

            var result = await _service.GetCourseByIdAsync(courseId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCoursesByTitleAsync_ValidTitle_ReturnsCourses()
        {
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "C# Programming" },
                new Course { Id = Guid.NewGuid(), Name = "Advanced C#" }
            };

            var mockDbSet = courses.AsQueryable().BuildMockDbSet();
            _mockCourseRepo.Setup(r => r.GetCourses()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByTitleAsync("C#");

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCoursesByTitleAsync_EmptyTitle_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCoursesByTitleAsync(string.Empty));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCoursesByTitleAsync(null!));
        }

        [Fact]
        public async Task GetCoursesByPriceRangeAsync_ValidRange_ReturnsCourses()
        {
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1", Price = 50 },
                new Course { Id = Guid.NewGuid(), Name = "Course 2", Price = 100 }
            };

            var mockDbSet = courses.AsQueryable().BuildMockDbSet();
            _mockCourseRepo.Setup(r => r.GetCourses()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByPriceRangeAsync(40, 80);

            Assert.Single(result);
            Assert.Equal("Course 1", result.First().Name);
        }

        [Fact]
        public async Task GetCoursesByPriceRangeAsync_InvalidRange_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCoursesByPriceRangeAsync(-1, 100));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCoursesByPriceRangeAsync(100, 50));
        }

        [Fact]
        public async Task GetCoursesByAverageRatingAsync_ValidRating_ReturnsCourses()
        {
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1", Reviews = new List<Review> { new Review { Rating = 4 }, new Review { Rating = 5 } } },
                new Course { Id = Guid.NewGuid(), Name = "Course 2", Reviews = new List<Review> { new Review { Rating = 5 } } },
                new Course { Id = Guid.NewGuid(), Name = "Course 3", Reviews = new List<Review> { new Review { Rating = 3 } } }
            };

            var mockDbSet = courses.AsQueryable().BuildMockDbSet();
            _mockCourseRepo.Setup(r => r.GetCourses()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByAverageRatingAsync(4.5);

            Assert.Equal(2, result.Count);
            Assert.All(result, c => Assert.True(c.Reviews.Average(r => r.Rating) >= 4.5));
        }

        [Fact]
        public async Task GetCoursesByCreatorAsync_ValidCreatorId_ReturnsCourses()
        {
            var creatorId = Guid.NewGuid();
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1", Stages = new List<Stage> { new Stage { CourseId = creatorId } } }
            };

            var mockDbSet = courses.AsQueryable().BuildMockDbSet();
            _mockCourseRepo.Setup(r => r.GetCourses()).Returns(mockDbSet.Object);

            var result = await _service.GetCoursesByCreatorAsync(creatorId);

            Assert.Single(result);
            Assert.Equal("Course 1", result.First().Name);
        }

        [Fact]
        public async Task GetCoursesByCreatorAsync_EmptyCreatorId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCoursesByCreatorAsync(Guid.Empty));
        }

        [Fact]
        public async Task AddCourseAsync_ValidCourse_ReturnsCourse()
        {
            var course = new Course { Id = Guid.NewGuid(), Name = "New Course" };
            _mockCourseRepo.Setup(r => r.AddCourse(course)).Returns(course);

            var result = await _service.AddCourseAsync(course);

            Assert.Equal(course.Id, result.Id);
            Assert.Equal(course.Name, result.Name);
            _mockCourseRepo.Verify(r => r.AddCourse(course), Times.Once);
        }

        [Fact]
        public async Task AddCourseAsync_NullCourse_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddCourseAsync(null!));
        }

        [Fact]
        public async Task AddCourseAsync_RepositoryReturnsNull_ThrowsInvalidOperationException()
        {
            var validCourse = new Course
            {
                Id = Guid.NewGuid(),
                Name = "Test Course",
                Reviews = new List<Review>()
            };

            _mockCourseRepo.Setup(repo => repo.AddCourse(It.IsAny<Course>())).Returns((Course)null);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddCourseAsync(validCourse));

            Assert.Equal("Failed to add course. Repository returned null.", exception.Message);
        }

        [Fact]
        public async Task UpdateCourseAsync_ValidCourse_ReturnsUpdatedCourse()
        {
            var course = new Course { Id = Guid.NewGuid(), Name = "Updated Course" };
            _mockCourseRepo.Setup(r => r.UpdateCourse(course)).Returns(course);

            var result = await _service.UpdateCourseAsync(course);

            Assert.Equal(course.Id, result!.Id);
            Assert.Equal(course.Name, result.Name);
            _mockCourseRepo.Verify(r => r.UpdateCourse(course), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseAsync_NullCourse_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateCourseAsync(null!));
        }

        [Fact]
        public async Task DeleteCourseAsync_ValidId_ReturnsDeletedCourse()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };
            _mockCourseRepo.Setup(r => r.DeleteCourse(courseId)).Returns(course);

            var result = await _service.DeleteCourseAsync(courseId);

            Assert.Equal(courseId, result!.Id);
            _mockCourseRepo.Verify(r => r.DeleteCourse(courseId), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_InvalidId_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteCourseAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetCourseByIdAsync_UsingStub_ReturnsFixedCourse()
        {
            var expectedCourse = new Course { Id = Guid.NewGuid(), Name = "Stubbed Course" };
            var stubRepo = new StubCourseRepository(expectedCourse);
            var service = new CourseService(stubRepo);

            var result = await service.GetCourseByIdAsync(expectedCourse.Id);

            Assert.NotNull(result);
            Assert.Equal(expectedCourse.Id, result!.Id);
        }

        [Fact]
        public async Task AddCourseAsync_UsingSpy_VerifiesAddWasCalled()
        {
            var spyRepo = new SpyCourseRepository();
            var service = new CourseService(spyRepo);
            var newCourse = new Course { Id = Guid.NewGuid(), Name = "Spy Course" };

            await service.AddCourseAsync(newCourse);

            Assert.True(spyRepo.WasAddCalled);
            Assert.Equal(newCourse.Id, spyRepo.LastAddedCourse!.Id);
        }

        [Fact]
        public async Task DeleteCourseAsync_UsingManualMock_VerifiesCallAndReturnsCourse()
        {
            var courseId = Guid.NewGuid();
            var mockRepo = new ManualMockCourseRepository(courseId);
            var service = new CourseService(mockRepo);

            var result = await service.DeleteCourseAsync(courseId);

            Assert.True(mockRepo.DeleteCalled);
            Assert.Equal(courseId, result!.Id);
        }
        private class DummyCourseRepository : ICourseRepository
        {
            public Course AddCourse(Course course) => throw new NotImplementedException();
            public Course? DeleteCourse(Guid courseId) => throw new NotImplementedException();
            public Course? GetCourseById(Guid courseId) => throw new NotImplementedException();
            public IQueryable<Course> GetCourses() => Enumerable.Empty<Course>().AsQueryable();
            public Course? UpdateCourse(Course course) => throw new NotImplementedException();
        }

        private class StubCourseRepository : ICourseRepository
        {
            private readonly Course _fixedCourse;

            public StubCourseRepository(Course fixedCourse)
            {
                _fixedCourse = fixedCourse;
            }

            public Course AddCourse(Course course) => course;
            public Course? DeleteCourse(Guid courseId) => null;
            public Course? GetCourseById(Guid courseId) => _fixedCourse;
            public IQueryable<Course> GetCourses() => Enumerable.Empty<Course>().AsQueryable();
            public Course? UpdateCourse(Course course) => course;
        }

        private class FakeCourseRepository : ICourseRepository
        {
            private readonly List<Course> _courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Fake 1" },
                new Course { Id = Guid.NewGuid(), Name = "Fake 2" }
            };

            public Course AddCourse(Course course)
            {
                _courses.Add(course);
                return course;
            }

            public Course? DeleteCourse(Guid courseId)
            {
                var course = _courses.FirstOrDefault(c => c.Id == courseId);
                if (course != null) _courses.Remove(course);
                return course;
            }

            public Course? GetCourseById(Guid courseId) => _courses.FirstOrDefault(c => c.Id == courseId);
            public IQueryable<Course> GetCourses() => _courses.AsQueryable();
            public Course? UpdateCourse(Course course)
            {
                var index = _courses.FindIndex(c => c.Id == course.Id);
                if (index != -1)
                {
                    _courses[index] = course;
                    return course;
                }
                return null;
            }
        }

        private class SpyCourseRepository : ICourseRepository
        {
            public bool WasAddCalled { get; private set; } = false;
            public Course? LastAddedCourse { get; private set; }

            public Course AddCourse(Course course)
            {
                WasAddCalled = true;
                LastAddedCourse = course;
                return course;
            }

            public Course? DeleteCourse(Guid courseId) => null;
            public Course? GetCourseById(Guid courseId) => null;
            public IQueryable<Course> GetCourses() => Enumerable.Empty<Course>().AsQueryable();
            public Course? UpdateCourse(Course course) => null;
        }

        private class ManualMockCourseRepository : ICourseRepository
        {
            private readonly Guid _expectedId;
            public bool DeleteCalled { get; private set; } = false;

            public ManualMockCourseRepository(Guid expectedId)
            {
                _expectedId = expectedId;
            }

            public Course AddCourse(Course course) => course;

            public Course? DeleteCourse(Guid courseId)
            {
                if (courseId == _expectedId)
                {
                    DeleteCalled = true;
                    return new Course { Id = courseId };
                }
                return null;
            }

            public Course? GetCourseById(Guid courseId) => null;
            public IQueryable<Course> GetCourses() => Enumerable.Empty<Course>().AsQueryable();
            public Course? UpdateCourse(Course course) => course;
        }
    }
}
