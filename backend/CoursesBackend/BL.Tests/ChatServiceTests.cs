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

namespace BL.Tests
{
    public class ChatServiceTests
    {
        private readonly Mock<IChatRepository> _mockChatRepository;
        private readonly ChatService _chatService;

        public ChatServiceTests()
        {
            _mockChatRepository = new Mock<IChatRepository>();
            _chatService = new ChatService(_mockChatRepository.Object);
        }

        [Fact]
        public async Task GetAllChatsAsync_ReturnsAllChats()
        {
            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid(), Name = "General" },
                new Chat { Id = Guid.NewGuid(), Name = "Support" }
            };
            
            var mockDbSet = chats.AsQueryable().BuildMockDbSet();
            _mockChatRepository.Setup(r => r.GetChats()).Returns(mockDbSet.Object);

            var result = await _chatService.GetAllChatsAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "General");
            Assert.Contains(result, c => c.Name == "Support");
        }

        [Fact]
        public async Task GetChatByIdAsync_ExistingId_ReturnsChat()
        {
            var chatId = Guid.NewGuid();
            var chat = new Chat { Id = chatId, Name = "General Chat" };

            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);

            var result = await _chatService.GetChatByIdAsync(chatId);

            Assert.NotNull(result);
            Assert.Equal(chatId, result.Id);
            Assert.Equal("General Chat", result.Name);
        }

        [Fact]
        public async Task GetChatByIdAsync_NonExistingId_ReturnsNull()
        {
            var chatId = Guid.NewGuid();
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns((Chat)null);

            var result = await _chatService.GetChatByIdAsync(chatId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddChatAsync_AddsAndReturnsChat()
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = "New Chat"
            };

            _mockChatRepository.Setup(r => r.AddChat(It.IsAny<Chat>())).Returns(chat);

            var result = await _chatService.AddChatAsync(chat);

            Assert.Equal(chat.Id, result.Id);
            Assert.Equal(chat.Name, result.Name);
            _mockChatRepository.Verify(r => r.AddChat(chat), Times.Once);
        }

        [Fact]
        public async Task DeleteChatAsync_ExistingId_DeletesAndReturnsChat()
        {
            var chatId = Guid.NewGuid();
            var chat = new Chat { Id = chatId, Name = "Chat to delete" };

            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);
            _mockChatRepository.Setup(r => r.DeleteChat(chatId)).Returns(chat);

            var result = await _chatService.DeleteChatAsync(chatId);

            Assert.NotNull(result);
            Assert.Equal(chatId, result.Id);
            _mockChatRepository.Verify(r => r.DeleteChat(chatId), Times.Once);
        }

        [Fact]
        public async Task DeleteChatAsync_NonExistingId_ReturnsNull()
        {
            var chatId = Guid.NewGuid();
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns((Chat)null);

            var result = await _chatService.DeleteChatAsync(chatId);

            Assert.Null(result);
            _mockChatRepository.Verify(r => r.DeleteChat(chatId), Times.Never);
        }

        [Fact]
        public async Task GetUsersInChatAsync_ExistingChatWithUsers_ReturnsUsers()
        {
            var chatId = Guid.NewGuid();
            var user1 = new User { Id = Guid.NewGuid(), FirstName = "Maciej", LastName = "Rzepka", Email = "maciej@example.com" };
            var user2 = new User { Id = Guid.NewGuid(), FirstName = "Julia", LastName = "Kowal", Email = "julia@example.com" };
            
            var chatUsers = new List<ChatUser>
            {
                new ChatUser { UserId = user1.Id, ChatId = chatId, User = user1 },
                new ChatUser { UserId = user2.Id, ChatId = chatId, User = user2 }
            };
            
            var chat = new Chat 
            { 
                Id = chatId, 
                Name = "Group Chat",
                Users = chatUsers 
            };

            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);

            var result = await _chatService.GetUsersInChatAsync(chatId);

            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Id == user1.Id);
            Assert.Contains(result, u => u.Id == user2.Id);
        }

        [Fact]
        public async Task GetUsersInChatAsync_ChatWithNoUsers_ReturnsEmptyCollection()
        {
            var chatId = Guid.NewGuid();
            var chat = new Chat 
            { 
                Id = chatId, 
                Name = "Empty Chat",
                Users = new List<ChatUser>() 
            };

            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);

            var result = await _chatService.GetUsersInChatAsync(chatId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsersInChatAsync_NonExistingChat_ReturnsEmptyCollection()
        {
            var chatId = Guid.NewGuid();
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns((Chat)null);

            var result = await _chatService.GetUsersInChatAsync(chatId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task RenameChatAsync_ExistingChat_UpdatesName()
        {
            var chatId = Guid.NewGuid();
            var chat = new Chat { Id = chatId, Name = "Old Name" };
            var newName = "New Name";

            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);
            _mockChatRepository.Setup(r => r.UpdateChat(It.IsAny<Chat>())).Returns(chat);

            await _chatService.RenameChatAsync(chatId, newName);

            Assert.Equal(newName, chat.Name);
            _mockChatRepository.Verify(r => r.UpdateChat(It.Is<Chat>(c => 
                c.Id == chatId && c.Name == newName)), Times.Once);
        }

        [Fact]
        public async Task RenameChatAsync_NonExistingChat_DoesNotCallUpdate()
        {
            var chatId = Guid.NewGuid();
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns((Chat)null);

            await _chatService.RenameChatAsync(chatId, "New Name");

            _mockChatRepository.Verify(r => r.UpdateChat(It.IsAny<Chat>()), Times.Never);
        }

        [Fact]
        public async Task ChatExistsAsync_ExistingChat_ReturnsTrue()
        {
            var chatId = Guid.NewGuid();
            var chat = new Chat { Id = chatId, Name = "Test Chat" };
            
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns(chat);

            var result = await _chatService.ChatExistsAsync(chatId);

            Assert.True(result);
        }

        [Fact]
        public async Task ChatExistsAsync_NonExistingChat_ReturnsFalse()
        {
            var chatId = Guid.NewGuid();
            _mockChatRepository.Setup(r => r.GetChatById(chatId)).Returns((Chat)null);

            var result = await _chatService.ChatExistsAsync(chatId);

            Assert.False(result);
        }
    }
}
