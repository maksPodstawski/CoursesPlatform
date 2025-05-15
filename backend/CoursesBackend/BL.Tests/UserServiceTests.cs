using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using MockQueryable.Moq;
using Xunit;
using BL.Services;

namespace BL.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPurchasedCoursesRepository> _mockPurchasedCoursesRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPurchasedCoursesRepository = new Mock<IPurchasedCoursesRepository>();
            _userService = new UserService(_mockUserRepository.Object, _mockPurchasedCoursesRepository.Object);
        }

        [Fact]
        public async Task GetUserCountAsync_ReturnsCorrectCount()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = "Kowalski", Email = "adam@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com" }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetUserCountAsync();

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = "Kowalski", Email = "adam@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com" }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Email == "adam@example.com");
            Assert.Contains(result, u => u.Email == "anna@example.com");
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "Adam", LastName = "Kowalski", Email = "adam@example.com" };

            _mockUserRepository.Setup(r => r.GetUserByID(userId)).Returns(user);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("Adam", result.FirstName);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.GetUserByID(userId)).Returns((User)null);

            var result = await _userService.GetUserByIdAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
        {
            var email = "adam@example.com";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = "Kowalski", Email = email }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetUserByEmailAsync(email);

            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_NonExistingEmail_ReturnsNull()
        {
            var email = "nieistniejacy@example.com";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = "Kowalski", Email = "adam@example.com" }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetUserByEmailAsync(email);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUsersByFirstNameAsync_ExistingName_ReturnsUsers()
        {
            var firstName = "Piotr";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = firstName, LastName = "Kowalski", Email = "piotr1@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = firstName, LastName = "Nowak", Email = "piotr2@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Karolina", LastName = "Kowalski", Email = "karolina@example.com" }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetUsersByFirstNameAsync(firstName);

            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.Equal(firstName, u.FirstName));
        }

        [Fact]
        public async Task GetUsersByLastNameAsync_ExistingName_ReturnsUsers()
        {
            var lastName = "Kowalski";
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = lastName, Email = "adam@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Karolina", LastName = lastName, Email = "karolina@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Tomasz", LastName = "Wiśniewski", Email = "tomasz@example.com" }
            };
            
            var mockDbSet = users.AsQueryable().BuildMockDbSet();
            _mockUserRepository.Setup(r => r.GetUsers()).Returns(mockDbSet.Object);

            var result = await _userService.GetUsersByLastNameAsync(lastName);

            Assert.Equal(2, result.Count);
            Assert.All(result, u => Assert.Equal(lastName, u.LastName));
        }

        [Fact]
        public async Task GetUsersByCourseIdAsync_ExistingCourseId_ReturnsUsers()
        {
            var courseId = Guid.NewGuid();
            var user1 = new User { Id = Guid.NewGuid(), FirstName = "Adam", LastName = "Kowalski", Email = "adam@example.com" };
            var user2 = new User { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com" };
            
            var purchasedCourses = new List<PurchasedCourses>
            {
                new PurchasedCourses { UserId = user1.Id, CourseId = courseId, User = user1 },
                new PurchasedCourses { UserId = user2.Id, CourseId = courseId, User = user2 },
                new PurchasedCourses { UserId = user1.Id, CourseId = Guid.NewGuid(), User = user1 }
            };
            
            var mockDbSet = purchasedCourses.AsQueryable().BuildMockDbSet();
            _mockPurchasedCoursesRepository.Setup(r => r.GetPurchasedCourses()).Returns(mockDbSet.Object);

            var result = await _userService.GetUsersByCourseIdAsync(courseId);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.Id == user1.Id);
            Assert.Contains(result, u => u.Id == user2.Id);
        }

        [Fact]
        public async Task AddUserAsync_AddsAndReturnsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Marek",
                LastName = "Lewandowski",
                Email = "marek@example.com"
            };

            _mockUserRepository.Setup(r => r.AddUser(It.IsAny<User>())).Returns(user);

            var result = await _userService.AddUserAsync(user);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
            _mockUserRepository.Verify(r => r.AddUser(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesAndReturnsUser()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Agnieszka",
                LastName = "Dąbrowska",
                Email = "agnieszka@example.com"
            };

            _mockUserRepository.Setup(r => r.UpdateUser(It.IsAny<User>())).Returns(user);

            var result = await _userService.UpdateUserAsync(user);

            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.Email, result.Email);
            _mockUserRepository.Verify(r => r.UpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingId_DeletesAndReturnsUser()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, FirstName = "Jakub", LastName = "Zieliński", Email = "jakub@example.com" };

            _mockUserRepository.Setup(r => r.DeleteUser(userId)).Returns(user);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            _mockUserRepository.Verify(r => r.DeleteUser(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingId_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(r => r.DeleteUser(userId)).Returns((User)null);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.Null(result);
            _mockUserRepository.Verify(r => r.DeleteUser(userId), Times.Once);
        }
    }
}
