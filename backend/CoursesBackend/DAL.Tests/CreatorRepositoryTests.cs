using DAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class CreatorRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetCreators_ShouldReturnAllCreators()
        {
            var options = CreateNewContextOptions();

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = new User { Id = userId1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var user2 = new User { Id = userId2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" };

            var testCreators = new List<Creator>
            {
                new Creator { Id = Guid.NewGuid(), UserId = userId1, User = user1 },
                new Creator { Id = Guid.NewGuid(), UserId = userId2, User = user2 }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.AddRange(user1, user2);
                context.Creators.AddRange(testCreators);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CreatorRepository(context);
                var result = repo.GetCreators().ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, c => c.User.FirstName == "John");
                Assert.Contains(result, c => c.User.FirstName == "Jane");
            }
        }

        [Fact]
        public void GetCreatorByID_ShouldReturnCorrectCreator()
        {
            var options = CreateNewContextOptions();

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var creatorId = Guid.NewGuid();
            var creator = new Creator { Id = creatorId, UserId = userId, User = user };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Creators.Add(creator);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CreatorRepository(context);
                var result = repo.GetCreatorByID(creatorId);

                Assert.NotNull(result);
                Assert.Equal(creatorId, result.Id);
                Assert.Equal("John", result.User.FirstName);
            }
        }

        [Fact]
        public void GetCreatorByID_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CreatorRepository(context);

            // Act
            var creator = repository.GetCreatorByID(Guid.NewGuid());

            // Assert
            Assert.Null(creator);
        }

        [Fact]
        public void AddCreator_ShouldAddCreatorToDatabase()
        {
            var options = CreateNewContextOptions();

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var creatorId = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CreatorRepository(context);
                var creator = new Creator { Id = creatorId, UserId = userId };
                var result = repo.AddCreator(creator);

                Assert.NotNull(result);
                Assert.Equal(userId, result.UserId);
                Assert.Equal("John", result.User.FirstName);
            }
        }

        [Fact]
        public void UpdateCreator_ShouldUpdateExistingCreator()
        {
            var options = CreateNewContextOptions();

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var user1 = new User { Id = userId1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var user2 = new User { Id = userId2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" };
            var creatorId = Guid.NewGuid();

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.AddRange(user1, user2);
                context.Creators.Add(new Creator { Id = creatorId, UserId = userId1 });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CreatorRepository(context);
                var result = repo.UpdateCreator(new Creator { Id = creatorId, UserId = userId2 });

                Assert.NotNull(result);
                Assert.Equal(userId2, result.UserId);
                Assert.NotNull(result.User);
                Assert.Equal("Jane", result.User.FirstName);
            }
        }

        [Fact]
        public void UpdateCreator_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CreatorRepository(context);
            var updatedCreator = new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };

            // Act
            var result = repository.UpdateCreator(updatedCreator);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DeleteCreator_ShouldRemoveCreatorFromDatabase()
        {
            var options = CreateNewContextOptions();

            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var creatorId = Guid.NewGuid();
            var creator = new Creator { Id = creatorId, UserId = userId, User = user };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Creators.Add(creator);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new CreatorRepository(context);
                var result = repo.DeleteCreator(creatorId);

                Assert.NotNull(result);
                Assert.Equal(creatorId, result.Id);
                Assert.Equal("John", result.User.FirstName);

                Assert.Empty(context.Creators);
            }
        }

        [Fact]
        public void DeleteCreator_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CreatorRepository(context);

            // Act
            var result = repository.DeleteCreator(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
