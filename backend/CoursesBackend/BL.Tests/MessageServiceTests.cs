using IDAL;
using Model;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Services;

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

        public class DummyMessageRepository : IMessageRepository
        {
            public IQueryable<Message> GetMessages()
            {
                return Enumerable.Empty<Message>().AsQueryable(); 
            }

            public Message? GetMessageById(Guid messageId)
            {
                return null; 
            }

            public Message AddMessage(Message message)
            {
                return message; 
            }

            public Message? UpdateMessage(Message message)
            {
                return message;
            }

            public Message? DeleteMessage(Guid messageId)
            {
                return null; 
            }
        }

        public class StubMessageRepository : IMessageRepository
        {
            public IQueryable<Message> GetMessages()
            {
                return new List<Message>
                {
                    new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Stub message 1", IsDeleted = false },
                    new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), Content = "Stub message 2", IsDeleted = true }
                }.AsQueryable();
            }

            public Message? GetMessageById(Guid messageId)
            {
                return new Message { Id = messageId, Content = "Stub message", IsDeleted = false }; 
            }

            public Message AddMessage(Message message)
            {
                return new Message { Id = Guid.NewGuid(), Content = "Stubbed new message", IsDeleted = false };
            }

            public Message? UpdateMessage(Message message)
            {
                return message; 
            }

            public Message? DeleteMessage(Guid messageId)
            {
                return new Message { Id = messageId, IsDeleted = true };
            }
        }

        public class FakeMessageRepository : IMessageRepository
        {
            private readonly List<Message> _messages = new List<Message>();

            public IQueryable<Message> GetMessages()
            {
                return _messages.AsQueryable(); 
            }

            public Message? GetMessageById(Guid messageId)
            {
                return _messages.FirstOrDefault(m => m.Id == messageId);
            }

            public Message AddMessage(Message message)
            {
                message.Id = Guid.NewGuid(); 
                _messages.Add(message);
                return message; 
            }

            public Message? UpdateMessage(Message message)
            {
                var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);
                if (existingMessage != null)
                {
                    existingMessage.Content = message.Content; 
                    return existingMessage;
                }
                return null; 
            }

            public Message? DeleteMessage(Guid messageId)
            {
                var message = _messages.FirstOrDefault(m => m.Id == messageId);
                if (message != null)
                {
                    message.IsDeleted = true; 
                    return message;
                }
                return null;
            }
        }

        public class MockMessageRepository : IMessageRepository
        {
            private readonly Mock<IMessageRepository> _mockRepo;

            public MockMessageRepository()
            {
                _mockRepo = new Mock<IMessageRepository>();
                _mockRepo.Setup(r => r.GetMessages()).Returns(new List<Message>
                {
                    new Message { Id = Guid.NewGuid(), Content = "Mock message", IsDeleted = false }
                }.AsQueryable());
                _mockRepo.Setup(r => r.GetMessageById(It.IsAny<Guid>())).Returns(new Message { Content = "Mocked message", IsDeleted = false });
            }

            public IQueryable<Message> GetMessages()
            {
                return _mockRepo.Object.GetMessages();
            }

            public Message? GetMessageById(Guid messageId)
            {
                return _mockRepo.Object.GetMessageById(messageId);
            }

            public Message AddMessage(Message message)
            {
                _mockRepo.Setup(r => r.AddMessage(It.IsAny<Message>())).Returns(message); 
                return _mockRepo.Object.AddMessage(message);
            }

            public Message? UpdateMessage(Message message)
            {
                _mockRepo.Setup(r => r.UpdateMessage(It.IsAny<Message>())).Returns(message); 
                return _mockRepo.Object.UpdateMessage(message);
            }

            public Message? DeleteMessage(Guid messageId)
            {
                _mockRepo.Setup(r => r.DeleteMessage(It.IsAny<Guid>())).Returns(new Message { Id = messageId, IsDeleted = true });
                return _mockRepo.Object.DeleteMessage(messageId);
            }
        }
        public class SpyMessageRepository : IMessageRepository
        {
            private readonly List<Message> _messages = new List<Message>();
            private readonly List<string> _calledMethods = new List<string>();

            public IQueryable<Message> GetMessages()
            {
                _calledMethods.Add("GetMessages");
                return _messages.AsQueryable();
            }

            public Message? GetMessageById(Guid messageId)
            {
                _calledMethods.Add("GetMessageById");
                return _messages.FirstOrDefault(m => m.Id == messageId);
            }

            public Message AddMessage(Message message)
            {
                _calledMethods.Add("AddMessage");
                message.Id = Guid.NewGuid();
                _messages.Add(message);
                return message;
            }

            public Message? UpdateMessage(Message message)
            {
                _calledMethods.Add("UpdateMessage");
                var existingMessage = _messages.FirstOrDefault(m => m.Id == message.Id);
                if (existingMessage != null)
                {
                    existingMessage.Content = message.Content;
                    return existingMessage;
                }
                return null;
            }

            public Message? DeleteMessage(Guid messageId)
            {
                _calledMethods.Add("DeleteMessage");
                var message = _messages.FirstOrDefault(m => m.Id == messageId);
                if (message != null)
                {
                    message.IsDeleted = true;
                    return message;
                }
                return null;
            }

            public List<string> GetCalledMethods()
            {
                return _calledMethods;
            }
        }
    }
}
