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
        [Fact]
        public void GetUserByID_WhenUserDoesNotExist_ReturnsNull()
        {
            var options = new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: "GetUserByID_WhenUserDoesNotExist_ReturnsNull")
                .Options;

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                var dummyGuid = Guid.NewGuid();

                var result = repository.GetUserByID(dummyGuid);

                Assert.Null(result);
            }
        }

        [Fact]
        public void GetUsers_ReturnsAllUsers()
        {
            var options = new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: "GetUsers_ReturnsAllUsers")
                .Options;

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

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                var result = repository.GetUsers().ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, u => u.Email == "maciej@example.com");
                Assert.Contains(result, u => u.Email == "jan@example.com");
            }
        }

        [Fact]
        public void AddUser_SavesUserToDatabase()
        {
            var options = new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: "AddUser_SavesUserToDatabase")
                .Options;

            var newUser = new User 
            { 
                Id = Guid.NewGuid(), 
                FirstName = "Marek", 
                LastName = "Towarek", 
                Email = "marek@example.com" 
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new UserRepository(context);
                repository.AddUser(newUser);
            }

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

            var options = new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: "UpdateUser_CallsSaveChanges")
                .Options;

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(existingUser);
                context.SaveChanges();
            }

            var contextSpy = new DbContextSpy(options);
            var repository = new UserRepository(contextSpy);

            var result = repository.UpdateUser(updatedUser);

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
            var options = new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: "DeleteUser_WhenUserExists_RemovesUserAndReturnsUser")
                .Options;

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

            var contextSpy = new DbContextSpy(options);
            var repository = new UserRepository(contextSpy);

            var result = repository.DeleteUser(userId);

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
