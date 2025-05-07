using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class MessageRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetMessageById_WhenMessageDoesNotExist_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using var context = new CoursesPlatformContext(options);
            var repository = new MessageRepository(context);

            var result = repository.GetMessageById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public void GetMessages_ReturnsAllMessages()
        {
            var options = CreateNewContextOptions();
            var testMessages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    ChatId = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Content = "Hello!",
                    CreatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Id = Guid.NewGuid(),
                    ChatId = Guid.NewGuid(),
                    AuthorId = Guid.NewGuid(),
                    Content = "Hi!",
                    CreatedAt = DateTime.UtcNow
                }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Messages.AddRange(testMessages);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new MessageRepository(context);
                var result = repository.GetMessages().ToList();

                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void AddMessage_SavesMessageToDatabase()
        {
            var options = CreateNewContextOptions();
            var newMessage = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Content = "This is a new message",
                CreatedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new MessageRepository(context);
                repository.AddMessage(newMessage);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var saved = context.Messages.Single();
                Assert.Equal(newMessage.Content, saved.Content);
            }
        }

        [Fact]
        public void UpdateMessage_CallsSaveChanges()
        {
            var options = CreateNewContextOptions();
            var messageId = Guid.NewGuid();
            var original = new Message
            {
                Id = messageId,
                ChatId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Content = "Original",
                CreatedAt = DateTime.UtcNow
            };
            var updated = new Message
            {
                Id = messageId,
                ChatId = original.ChatId,
                AuthorId = original.AuthorId,
                Content = "Updated",
                CreatedAt = original.CreatedAt,
                EditedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Messages.Add(original);
                context.SaveChanges();
            }

            var spyContext = new DbContextSpy(options);
            var repository = new MessageRepository(spyContext);

            var result = repository.UpdateMessage(updated);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.NotNull(result);
            Assert.Equal("Updated", result?.Content);
        }

        [Fact]
        public void DeleteMessage_RemovesAndReturnsMessage()
        {
            var options = CreateNewContextOptions();
            var messageId = Guid.NewGuid();
            var toDelete = new Message
            {
                Id = messageId,
                ChatId = Guid.NewGuid(),
                AuthorId = Guid.NewGuid(),
                Content = "Delete me",
                CreatedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Messages.Add(toDelete);
                context.SaveChanges();
            }

            var spyContext = new DbContextSpy(options);
            var repository = new MessageRepository(spyContext);
            var deleted = repository.DeleteMessage(messageId);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.NotNull(deleted);
            Assert.Equal(messageId, deleted?.Id);

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Messages.ToList());
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
