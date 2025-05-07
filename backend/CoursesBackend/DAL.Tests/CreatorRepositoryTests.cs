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
        private DbContextOptions<CoursesPlatformContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetCreators_ShouldReturnAllCreators()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);

            context.Creators.AddRange(new List<Creator>
            {
                new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() },
                new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() }
            });
            context.SaveChanges();

            var repository = new CreatorRepository(context);

            // Act
            var creators = repository.GetCreators().ToList();

            // Assert
            Assert.Equal(2, creators.Count);
        }

        [Fact]
        public void GetCreatorByID_ShouldReturnCorrectCreator()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            var creatorId = Guid.NewGuid();

            using var context = new CoursesPlatformContext(options);
            context.Creators.Add(new Creator { Id = creatorId, UserId = Guid.NewGuid() });
            context.SaveChanges();

            var repository = new CreatorRepository(context);

            // Act
            var creator = repository.GetCreatorByID(creatorId);

            // Assert
            Assert.NotNull(creator);
            Assert.Equal(creatorId, creator.Id);
        }

        [Fact]
        public void GetCreatorByID_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
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
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CreatorRepository(context);
            var newCreator = new Creator { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };

            // Act
            var addedCreator = repository.AddCreator(newCreator);

            // Assert
            Assert.NotNull(addedCreator);
            Assert.Equal(newCreator.Id, addedCreator.Id);
            Assert.Single(context.Creators);
        }

        [Fact]
        public void UpdateCreator_ShouldUpdateExistingCreator()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var creatorId = Guid.NewGuid();
            context.Creators.Add(new Creator { Id = creatorId, UserId = Guid.NewGuid() });
            context.SaveChanges();

            var repository = new CreatorRepository(context);
            var updatedCreator = new Creator { Id = creatorId, UserId = Guid.NewGuid() };

            // Act
            var result = repository.UpdateCreator(updatedCreator);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedCreator.UserId, result.UserId);
        }

        [Fact]
        public void UpdateCreator_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
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
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var creatorId = Guid.NewGuid();
            context.Creators.Add(new Creator { Id = creatorId, UserId = Guid.NewGuid() });
            context.SaveChanges();

            var repository = new CreatorRepository(context);

            // Act
            var deletedCreator = repository.DeleteCreator(creatorId);

            // Assert
            Assert.NotNull(deletedCreator);
            Assert.Equal(creatorId, deletedCreator.Id);
            Assert.Empty(context.Creators);
        }

        [Fact]
        public void DeleteCreator_ShouldReturnNull_WhenCreatorDoesNotExist()
        {
            // Arrange
            var options = GetInMemoryDbOptions();
            using var context = new CoursesPlatformContext(options);
            var repository = new CreatorRepository(context);

            // Act
            var result = repository.DeleteCreator(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}
