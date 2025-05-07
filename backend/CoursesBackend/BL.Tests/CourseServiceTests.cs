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
    }
}
