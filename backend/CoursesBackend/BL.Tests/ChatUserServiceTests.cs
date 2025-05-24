using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Moq;
using MockQueryable.Moq;
using Xunit;
using BL.Services;

namespace BL.Tests
{
    public class ChatUserServiceTests
    {
        private readonly Mock<IChatUserRepository> _mockChatUserRepository;
        private readonly ChatUserService _chatUserService;

        public ChatUserServiceTests()
        {
            _mockChatUserRepository = new Mock<IChatUserRepository>();
            _chatUserService = new ChatUserService(_mockChatUserRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllChatUsers()
        {
            var chatUsers = new List<ChatUser>
            {
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = Guid.NewGuid(), 
                    UserId = Guid.NewGuid(), 
                    JoinedAt = DateTime.UtcNow.AddDays(-5) 
                },
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = Guid.NewGuid(), 
                    UserId = Guid.NewGuid(), 
                    JoinedAt = DateTime.UtcNow.AddDays(-2) 
                }
            };
            
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);

            var result = await _chatUserService.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, cu => cu.Id == chatUsers[0].Id);
            Assert.Contains(result, cu => cu.Id == chatUsers[1].Id);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsChatUser()
        {
            var chatUserId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUser = new ChatUser 
            { 
                Id = chatUserId, 
                ChatId = chatId, 
                UserId = userId, 
                JoinedAt = DateTime.UtcNow.AddDays(-3) 
            };

            _mockChatUserRepository.Setup(r => r.GetChatUserById(chatUserId)).Returns(chatUser);

            var result = await _chatUserService.GetByIdAsync(chatUserId);

            Assert.NotNull(result);
            Assert.Equal(chatUserId, result.Id);
            Assert.Equal(chatId, result.ChatId);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var chatUserId = Guid.NewGuid();
            _mockChatUserRepository.Setup(r => r.GetChatUserById(chatUserId)).Returns((ChatUser)null);

            var result = await _chatUserService.GetByIdAsync(chatUserId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddUserToChatAsync_UserNotInChat_AddsChatUserAndReturnsIt()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUsers = new List<ChatUser>();
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);
            _mockChatUserRepository.Setup(r => r.AddChatUser(It.IsAny<ChatUser>()))
                .Returns((ChatUser cu) => cu);

            var result = await _chatUserService.AddUserToChatAsync(chatId, userId);

            Assert.NotNull(result);
            Assert.Equal(chatId, result.ChatId);
            Assert.Equal(userId, result.UserId);
            _mockChatUserRepository.Verify(r => r.AddChatUser(It.Is<ChatUser>(cu => 
                cu.ChatId == chatId && cu.UserId == userId)), Times.Once);
        }

        [Fact]
        public async Task AddUserToChatAsync_UserAlreadyInChat_ReturnsNull()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUsers = new List<ChatUser>
            {
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = chatId, 
                    UserId = userId, 
                    JoinedAt = DateTime.UtcNow.AddDays(-2) 
                }
            };
            
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);

            var result = await _chatUserService.AddUserToChatAsync(chatId, userId);

            Assert.Null(result);
            _mockChatUserRepository.Verify(r => r.AddChatUser(It.IsAny<ChatUser>()), Times.Never);
        }

        [Fact]
        public async Task RemoveUserFromChatAsync_UserInChat_RemovesChatUserAndReturnsIt()
        {
            var chatUserId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUser = new ChatUser 
            { 
                Id = chatUserId, 
                ChatId = chatId, 
                UserId = userId, 
                JoinedAt = DateTime.UtcNow.AddDays(-3) 
            };
            
            var chatUsers = new List<ChatUser> { chatUser };
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);
            _mockChatUserRepository.Setup(r => r.DeleteChatUser(chatUserId)).Returns(chatUser);

            var result = await _chatUserService.RemoveUserFromChatAsync(chatId, userId);

            Assert.NotNull(result);
            Assert.Equal(chatUserId, result.Id);
            _mockChatUserRepository.Verify(r => r.DeleteChatUser(chatUserId), Times.Once);
        }

        [Fact]
        public async Task RemoveUserFromChatAsync_UserNotInChat_ReturnsNull()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUsers = new List<ChatUser>();
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);

            var result = await _chatUserService.RemoveUserFromChatAsync(chatId, userId);

            Assert.Null(result);
            _mockChatUserRepository.Verify(r => r.DeleteChatUser(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task IsUserInChatAsync_UserInChat_ReturnsTrue()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUsers = new List<ChatUser>
            {
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = chatId, 
                    UserId = userId, 
                    JoinedAt = DateTime.UtcNow.AddDays(-2) 
                }
            };
            
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);

            var result = await _chatUserService.IsUserInChatAsync(chatId, userId);

            Assert.True(result);
        }

        [Fact]
        public async Task IsUserInChatAsync_UserNotInChat_ReturnsFalse()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            
            var chatUsers = new List<ChatUser>
            {
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = Guid.NewGuid(),
                    UserId = userId, 
                    JoinedAt = DateTime.UtcNow.AddDays(-2) 
                },
                new ChatUser 
                { 
                    Id = Guid.NewGuid(), 
                    ChatId = chatId, 
                    UserId = Guid.NewGuid(),
                    JoinedAt = DateTime.UtcNow.AddDays(-1) 
                }
            };
            
            var mockDbSet = chatUsers.AsQueryable().BuildMockDbSet();
            _mockChatUserRepository.Setup(r => r.GetChatUsers()).Returns(mockDbSet.Object);

            var result = await _chatUserService.IsUserInChatAsync(chatId, userId);

            Assert.False(result);
        }
        public class DummyChatUserRepository : IChatUserRepository
        {
            public IQueryable<ChatUser> GetChatUsers() => Enumerable.Empty<ChatUser>().AsQueryable();
            public ChatUser? GetChatUserById(Guid chatUserId) => null;
            public ChatUser? AddChatUser(ChatUser chatUser) => null;
            public ChatUser? UpdateChatUser(ChatUser chatUser) => null;
            public ChatUser? DeleteChatUser(Guid chatUserId) => null;
        }

        public class StubChatUserRepository : IChatUserRepository
        {
            private readonly ChatUser _chatUser;

            public StubChatUserRepository(ChatUser chatUser)
            {
                _chatUser = chatUser;
            }

            public IQueryable<ChatUser> GetChatUsers() => new List<ChatUser> { _chatUser }.AsQueryable();
            public ChatUser? GetChatUserById(Guid chatUserId) => _chatUser.Id == chatUserId ? _chatUser : null;
            public ChatUser? AddChatUser(ChatUser chatUser) => chatUser;
            public ChatUser? UpdateChatUser(ChatUser chatUser) => chatUser;
            public ChatUser? DeleteChatUser(Guid chatUserId) => _chatUser.Id == chatUserId ? _chatUser : null;
        }

        public class FakeChatUserRepository : IChatUserRepository
        {
            private readonly List<ChatUser> _data = new();

            public IQueryable<ChatUser> GetChatUsers() => _data.AsQueryable();

            public ChatUser? GetChatUserById(Guid chatUserId) => _data.FirstOrDefault(cu => cu.Id == chatUserId);

            public ChatUser? AddChatUser(ChatUser chatUser)
            {
                _data.Add(chatUser);
                return chatUser;
            }

            public ChatUser? UpdateChatUser(ChatUser chatUser)
            {
                var index = _data.FindIndex(c => c.Id == chatUser.Id);
                if (index >= 0) _data[index] = chatUser;
                return chatUser;
            }

            public ChatUser? DeleteChatUser(Guid chatUserId)
            {
                var user = GetChatUserById(chatUserId);
                if (user != null)
                {
                    _data.Remove(user);
                    return user;
                }
                return null;
            }
        }

        public class SpyChatUserRepository : IChatUserRepository
        {
            public bool WasAddCalled { get; private set; } = false;
            public ChatUser? LastAdded { get; private set; }

            public IQueryable<ChatUser> GetChatUsers() => Enumerable.Empty<ChatUser>().AsQueryable();

            public ChatUser? GetChatUserById(Guid chatUserId) => null;

            public ChatUser? AddChatUser(ChatUser chatUser)
            {
                WasAddCalled = true;
                LastAdded = chatUser;
                return chatUser;
            }

            public ChatUser? UpdateChatUser(ChatUser chatUser) => null;

            public ChatUser? DeleteChatUser(Guid chatUserId) => null;
        }

        [Fact]
        public async Task AddUserToChatAsync_UsesSpyToVerifyAddCall()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var spyRepo = new SpyChatUserRepository();
            var service = new ChatUserService(spyRepo);

            var result = await service.AddUserToChatAsync(chatId, userId);

            Assert.True(spyRepo.WasAddCalled);
            Assert.NotNull(spyRepo.LastAdded);
            Assert.Equal(chatId, spyRepo.LastAdded.ChatId);
            Assert.Equal(userId, spyRepo.LastAdded.UserId);
        }

        [Fact]
        public async Task Constructor_WithDummyRepository_DoesNotThrow()
        {
            var dummyRepo = new DummyChatUserRepository();
            var service = new ChatUserService(dummyRepo);

            Assert.NotNull(service);
        }

        [Fact]
        public async Task GetByIdAsync_StubReturnsKnownUser()
        {
            var knownUser = new ChatUser
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                JoinedAt = DateTime.UtcNow
            };

            var stubRepo = new StubChatUserRepository(knownUser);
            var service = new ChatUserService(stubRepo);

            var result = await service.GetByIdAsync(knownUser.Id);

            Assert.NotNull(result);
            Assert.Equal(knownUser.Id, result.Id);
            Assert.Equal(knownUser.ChatId, result.ChatId);
        }
        [Fact]
        public async Task AddUserToChatAsync_UsingFake_StoresUser()
        {
            var fakeRepo = new FakeChatUserRepository();
            var service = new ChatUserService(fakeRepo);

            var chatId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var result = await service.AddUserToChatAsync(chatId, userId);

            Assert.NotNull(result);
            var storedUsers = fakeRepo.GetChatUsers().ToList();
            Assert.Single(storedUsers);
            Assert.Equal(chatId, storedUsers.First().ChatId);
            Assert.Equal(userId, storedUsers.First().UserId);
        }
    }
}
