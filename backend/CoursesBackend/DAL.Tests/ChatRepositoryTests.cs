using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Model;
using Xunit;

namespace DAL.Tests
{
    public class ChatRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetChats_ReturnsAllChats()
        {
            var options = CreateNewContextOptions();
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Chats.Add(new Chat { Id = Guid.NewGuid(), Name = "Test Chat 1" });
                context.Chats.Add(new Chat { Id = Guid.NewGuid(), Name = "Test Chat 2" });
                context.SaveChanges();
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var chats = repository.GetChats().ToList();
                
                Assert.Equal(2, chats.Count);
                Assert.Contains(chats, c => c.Name == "Test Chat 1");
                Assert.Contains(chats, c => c.Name == "Test Chat 2");
            }
        }

        [Fact]
        public void GetChatById_ExistingId_ReturnsChat()
        {
            var options = CreateNewContextOptions();
            var chatId = Guid.NewGuid();
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Chats.Add(new Chat { Id = chatId, Name = "Test Chat" });
                context.SaveChanges();
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var chat = repository.GetChatById(chatId);
                
                Assert.NotNull(chat);
                Assert.Equal(chatId, chat.Id);
                Assert.Equal("Test Chat", chat.Name);
            }
        }

        [Fact]
        public void GetChatById_NonExistingId_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var chat = repository.GetChatById(Guid.NewGuid());
                
                Assert.Null(chat);
            }
        }

        [Fact]
        public void AddChat_AddsNewChat()
        {
            var options = CreateNewContextOptions();
            var newChat = new Chat { Id = Guid.NewGuid(), Name = "New Chat" };
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var result = repository.AddChat(newChat);
                
                Assert.NotNull(result);
                Assert.Equal(newChat.Id, result.Id);
                Assert.Equal(newChat.Name, result.Name);
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Equal(1, context.Chats.Count());
                var chat = context.Chats.Single();
                Assert.Equal(newChat.Id, chat.Id);
                Assert.Equal(newChat.Name, chat.Name);
            }
        }

        [Fact]
        public void UpdateChat_ExistingChat_UpdatesChat()
        {
            var options = CreateNewContextOptions();
            var chatId = Guid.NewGuid();
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Chats.Add(new Chat { Id = chatId, Name = "Original Name" });
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                
                var existingChat = context.Chats.Find(chatId);
                existingChat.Name = "Updated Name";
                
                var result = repository.UpdateChat(existingChat);
                
                Assert.NotNull(result);
                Assert.Equal(chatId, result.Id);
                Assert.Equal("Updated Name", result.Name);
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                var chat = context.Chats.Single();
                Assert.Equal(chatId, chat.Id);
                Assert.Equal("Updated Name", chat.Name);
            }
        }

        [Fact]
        public void UpdateChat_NonExistingChat_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            var nonExistingChat = new Chat { Id = Guid.NewGuid(), Name = "Non-existing Chat" };
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var result = repository.UpdateChat(nonExistingChat);
                
                Assert.Null(result);
            }
        }

        [Fact]
        public void DeleteChat_ExistingChat_RemovesChat()
        {
            var options = CreateNewContextOptions();
            var chatId = Guid.NewGuid();
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Chats.Add(new Chat { Id = chatId, Name = "Test Chat" });
                context.SaveChanges();
            }
            

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var result = repository.DeleteChat(chatId);
                
                Assert.NotNull(result);
                Assert.Equal(chatId, result.Id);
                Assert.Equal("Test Chat", result.Name);
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Chats);
            }
        }

        [Fact]
        public void DeleteChat_NonExistingChat_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatRepository(context);
                var result = repository.DeleteChat(Guid.NewGuid());

                Assert.Null(result);
            }
        }
    }
}
