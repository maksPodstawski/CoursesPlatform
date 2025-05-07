using IDAL;
using Model;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Tests
{
    public class MessageServiceTests
    {
        private readonly Mock<IMessageRepository> _mockMessageRepository;
        private readonly MessageService _messageService;

        public MessageServiceTests()
        {
            _mockMessageRepository = new Mock<IMessageRepository>();
            _messageService = new MessageService(_mockMessageRepository.Object);
        }

        [Fact]
        public async Task GetMessagesByChatIdAsync_ReturnsMessages_ForGivenChat()
        {
            var chatId = Guid.NewGuid();
            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ChatId = chatId, Content = "Test 1", IsDeleted = false },
                new Message { Id = Guid.NewGuid(), ChatId = chatId, Content = "Test 2", IsDeleted = false },
                new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Wrong Chat", IsDeleted = false },
                new Message { Id = Guid.NewGuid(), ChatId = chatId, Content = "Deleted", IsDeleted = true }
            };

            var mockDbSet = messages.AsQueryable().BuildMockDbSet();
            _mockMessageRepository.Setup(r => r.GetMessages()).Returns(mockDbSet.Object);

            var result = await _messageService.GetMessagesByChatIdAsync(chatId);

            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Equal(chatId, m.ChatId));
            Assert.DoesNotContain(result, m => m.IsDeleted);
        }

        [Fact]
        public async Task GetMessageByIdAsync_ReturnsCorrectMessage()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, Content = "Test Message" };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(message);

            var result = await _messageService.GetMessageByIdAsync(messageId);

            Assert.NotNull(result);
            Assert.Equal(messageId, result?.Id);
        }

        [Fact]
        public async Task GetMessageByIdAsync_ReturnsNull_WhenNotFound()
        {
            var messageId = Guid.NewGuid();
            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns((Message?)null);

            var result = await _messageService.GetMessageByIdAsync(messageId);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddMessageAsync_SetsFieldsAndAddsMessage()
        {
            var message = new Message { Content = "New message", ChatId = Guid.NewGuid(), AuthorId = Guid.NewGuid() };

            _mockMessageRepository.Setup(r => r.AddMessage(It.IsAny<Message>()))
                                  .Returns<Message>(m => m);

            var result = await _messageService.AddMessageAsync(message);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.False(result.IsDeleted);
            Assert.True((DateTime.UtcNow - result.CreatedAt).TotalSeconds < 5);
            _mockMessageRepository.Verify(r => r.AddMessage(It.Is<Message>(m => m.Content == message.Content)), Times.Once);
        }

        [Fact]
        public async Task EditMessageAsync_UpdatesContentAndTimestamp_WhenExistsAndNotDeleted()
        {
            var messageId = Guid.NewGuid();
            var originalMessage = new Message
            {
                Id = messageId,
                Content = "Old content",
                IsDeleted = false
            };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(originalMessage);
            _mockMessageRepository.Setup(r => r.AddMessage(It.IsAny<Message>())).Returns<Message>(m => m);

            var result = await _messageService.EditMessageAsync(messageId, "New content");

            Assert.NotNull(result);
            Assert.Equal("New content", result?.Content);
            Assert.True(result?.EditedAt.HasValue);
        }

        [Fact]
        public async Task EditMessageAsync_ReturnsNull_WhenMessageIsDeleted()
        {
            var messageId = Guid.NewGuid();
            var deletedMessage = new Message { Id = messageId, IsDeleted = true };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(deletedMessage);

            var result = await _messageService.EditMessageAsync(messageId, "Ignored");

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMessageAsync_MarksAsDeleted_WhenMessageExistsAndNotAlreadyDeleted()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, IsDeleted = false };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(message);
            _mockMessageRepository.Setup(r => r.AddMessage(It.IsAny<Message>())).Returns<Message>(m => m);

            var result = await _messageService.DeleteMessageAsync(messageId);

            Assert.True(result?.IsDeleted);
        }

        [Fact]
        public async Task DeleteMessageAsync_ReturnsNull_WhenAlreadyDeleted()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, IsDeleted = true };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(message);

            var result = await _messageService.DeleteMessageAsync(messageId);

            Assert.Null(result);
        }

        [Fact]
        public async Task MessageExistsAsync_ReturnsTrue_WhenMessageExistsAndIsNotDeleted()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, IsDeleted = false };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(message);

            var result = await _messageService.MessageExistsAsync(messageId);

            Assert.True(result);
        }

        [Fact]
        public async Task MessageExistsAsync_ReturnsFalse_WhenMessageIsDeleted()
        {
            var messageId = Guid.NewGuid();
            var message = new Message { Id = messageId, IsDeleted = true };

            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns(message);

            var result = await _messageService.MessageExistsAsync(messageId);

            Assert.False(result);
        }

        [Fact]
        public async Task MessageExistsAsync_ReturnsFalse_WhenMessageDoesNotExist()
        {
            var messageId = Guid.NewGuid();
            _mockMessageRepository.Setup(r => r.GetMessageById(messageId)).Returns((Message?)null);

            var result = await _messageService.MessageExistsAsync(messageId);

            Assert.False(result);
        }
    }
}
