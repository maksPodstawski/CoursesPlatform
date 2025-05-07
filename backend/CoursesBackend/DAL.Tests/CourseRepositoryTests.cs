using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class CourseRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetCourses_ShouldReturnAllCourses()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            context.Courses.AddRange(new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1" },
                new Course { Id = Guid.NewGuid(), Name = "Course 2" }
            });
            context.SaveChanges();
            var repository = new CourseRepository(context);

            // Act
            var courses = repository.GetCourses().ToList();

            // Assert
            Assert.Equal(2, courses.Count);
        }

        [Fact]
        public void GetCourseById_ShouldReturnCorrectCourse()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            var courseId = Guid.NewGuid();

            using var context = new CoursesPlatformContext(options);
            context.Courses.Add(new Course { Id = courseId, Name = "Test Course", Price = 150 });
            context.SaveChanges();

            var repository = new CourseRepository(context);

            // Act
            var course = repository.GetCourseById(courseId);

            // Assert
            Assert.NotNull(course);
            Assert.Equal("Test Course", course.Name);
        }

        [Fact]
        public void GetCourseById_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CourseRepository(context);

            // Act
            var course = repository.GetCourseById(Guid.NewGuid());

            // Assert
            Assert.Null(course);
        }

        [Fact]
        public void AddCourse_ShouldAddCourseToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CourseRepository(context);
            var newCourse = new Course { Id = Guid.NewGuid(), Name = "New Course", Price = 300 };

            // Act
            var addedCourse = repository.AddCourse(newCourse);

            // Assert
            Assert.NotNull(addedCourse);
            Assert.Equal("New Course", addedCourse.Name);
            Assert.Single(context.Courses);
        }

        [Fact]
        public void UpdateCourse_ShouldUpdateExistingCourse()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Name = "Old Name", Price = 100 });
            context.SaveChanges();

            var repository = new CourseRepository(context);
            var updatedCourse = new Course { Id = courseId, Name = "Updated Name", Price = 150 };

            // Act
            var result = repository.UpdateCourse(updatedCourse);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal(150, result.Price);
        }

        [Fact]
        public void UpdateCourse_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CourseRepository(context);
            var updatedCourse = new Course { Id = Guid.NewGuid(), Name = "Nonexistent Course", Price = 200 };

            // Act
            var result = repository.UpdateCourse(updatedCourse);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteCourse_ShouldRemoveCourseFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var courseId = Guid.NewGuid();
            context.Courses.Add(new Course { Id = courseId, Name = "Course to Delete", Price = 100 });
            context.SaveChanges();

            var repository = new CourseRepository(context);

            // Act
            var deletedCourse = repository.DeleteCourse(courseId);

            // Assert
            Assert.NotNull(deletedCourse);
            Assert.Equal("Course to Delete", deletedCourse.Name);
            Assert.Empty(context.Courses);
        }

        [Fact]
        public void DeleteCourse_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CourseRepository(context);

            // Act
            var result = repository.DeleteCourse(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

    }
}
