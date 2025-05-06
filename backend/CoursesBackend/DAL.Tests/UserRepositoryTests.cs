using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using DAL;
using IDAL;
using Model;

namespace DAL.Tests
{
    public class UserRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetUserByID_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();

            // Act
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                var dummyGuid = Guid.NewGuid();

                var result = repository.GetUserByID(dummyGuid);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var testUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Maciej", LastName = "Rzepka", Email = "maciej@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Kowal", Email = "jan@example.com" }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.AddRange(testUsers);
                context.SaveChanges();
            }

            // Act
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                var result = repository.GetUsers().ToList();

                // Assert
                Assert.Equal(2, result.Count);
                Assert.Contains(result, u => u.Email == "maciej@example.com");
                Assert.Contains(result, u => u.Email == "jan@example.com");
            }
        }

        [Fact]
        public void AddUser_SavesUserToDatabase()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var newUser = new User 
            { 
                Id = Guid.NewGuid(), 
                FirstName = "Marek", 
                LastName = "Towarek", 
                Email = "marek@example.com" 
            };

            // Act
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                repository.AddUser(newUser);
            }

            // Assert
            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Equal(1, context.Users.Count());
                var savedUser = context.Users.Single();
                Assert.Equal(newUser.Id, savedUser.Id);
                Assert.Equal("Marek", savedUser.FirstName);
                Assert.Equal("Towarek", savedUser.LastName);
                Assert.Equal("marek@example.com", savedUser.Email);
            }
        }

        [Fact]
        public void UpdateUser_CallsSaveChanges()
        {
            // Arrange
            var options = CreateNewContextOptions();
            var userId = Guid.NewGuid();
            var existingUser = new User 
            { 
                Id = userId, 
                FirstName = "Bob", 
                LastName = "Budowniczy", 
                Email = "bob@example.com" 
            };
            
            var updatedUser = new User 
            { 
                Id = userId, 
                FirstName = "Robert", 
                LastName = "Brown", 
                Email = "robert@example.com" 
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(existingUser);
                context.SaveChanges();
            }

            // Act
            var contextSpy = new DbContextSpy(options);
            var repository = new UserRepository(contextSpy);

            var result = repository.UpdateUser(updatedUser);

            // Assert
            Assert.Equal(1, contextSpy.SaveChangesCallCount);
            Assert.NotNull(result);
            if (result != null)
            {
                Assert.Equal(updatedUser.FirstName, result.FirstName);
                Assert.Equal(updatedUser.Email, result.Email);
            }
        }

        [Fact]
        public void DeleteUser_WhenUserExists_RemovesUserAndReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();

            var userId = Guid.NewGuid();
            var userToDelete = new User 
            { 
                Id = userId, 
                FirstName = "Maciej", 
                LastName = "Rzepka", 
                Email = "maciej@example.com" 
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(userToDelete);
                context.SaveChanges();
            }

            // Act
            var contextSpy = new DbContextSpy(options);
            var repository = new UserRepository(contextSpy);

            var result = repository.DeleteUser(userId);

            // Assert
            Assert.Equal(userId, result?.Id);
            Assert.Equal("Maciej", result?.FirstName);
            Assert.Equal(1, contextSpy.SaveChangesCallCount);

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Users.ToList());
            }
        }

        private class DbContextSpy : CoursesPlatformContext
        {
            public int SaveChangesCallCount { get; private set; } = 0;

            public DbContextSpy(DbContextOptions<CoursesPlatformContext> options) : base(options) { }

            public override int SaveChanges()
            {
                SaveChangesCallCount++;
                return base.SaveChanges();
            }
        }
    }
}
