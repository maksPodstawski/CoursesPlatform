using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Model;

namespace DAL.Tests
{
    public class ChatUserRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetChatUserById_WhenChatUserDoesNotExist_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                var dummyGuid = Guid.NewGuid();

                var result = repository.GetChatUserById(dummyGuid);

                Assert.Null(result);
            }
        }

        [Fact]
        public void GetChatUsers_ReturnsAllChatUsers()
        {
            var options = CreateNewContextOptions();

            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var chatId = Guid.NewGuid();

            var user1 = new User { Id = userId1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@example.com" };
            var user2 = new User { Id = userId2, FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com" };
            var chat = new Chat { Id = chatId, Name = "Test Chat" };

            var testChatUsers = new List<ChatUser>
            {
                new ChatUser { Id = Guid.NewGuid(), UserId = userId1, ChatId = chatId, JoinedAt = DateTime.UtcNow.AddDays(-1) },
                new ChatUser { Id = Guid.NewGuid(), UserId = userId2, ChatId = chatId, JoinedAt = DateTime.UtcNow }
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.AddRange(user1, user2);
                context.Chats.Add(chat);
                context.SaveChanges();
                
                context.ChatUsers.AddRange(testChatUsers);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                var result = repository.GetChatUsers().ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, cu => cu.UserId == userId1);
                Assert.Contains(result, cu => cu.UserId == userId2);
            }
        }

        [Fact]
        public void AddChatUser_SavesChatUserToDatabase()
        {
            var options = CreateNewContextOptions();
            
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            
            var user = new User { Id = userId, FirstName = "Test", LastName = "User", Email = "test@example.com" };
            var chat = new Chat { Id = chatId, Name = "New Chat" };
            
            var newChatUser = new ChatUser 
            { 
                Id = Guid.NewGuid(), 
                UserId = userId,
                ChatId = chatId,
                JoinedAt = DateTime.UtcNow
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Chats.Add(chat);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                repository.AddChatUser(newChatUser);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Equal(1, context.ChatUsers.Count());
                var savedChatUser = context.ChatUsers.Single();
                Assert.Equal(newChatUser.Id, savedChatUser.Id);
                Assert.Equal(userId, savedChatUser.UserId);
                Assert.Equal(chatId, savedChatUser.ChatId);
            }
        }

        [Fact]
        public void UpdateChatUser_ExistingChatUser_UpdatesAndReturnsChatUser()
        {
            var options = CreateNewContextOptions();
            
            var userId = Guid.NewGuid();
            var chatId1 = Guid.NewGuid();
            var chatId2 = Guid.NewGuid();
            var chatUserId = Guid.NewGuid();
            
            var user = new User { Id = userId, FirstName = "Test", LastName = "User", Email = "test@example.com" };
            var chat1 = new Chat { Id = chatId1, Name = "Original Chat" };
            var chat2 = new Chat { Id = chatId2, Name = "New Chat" };
            
            var originalJoinDate = DateTime.UtcNow.AddDays(-5);
            var chatUser = new ChatUser 
            { 
                Id = chatUserId, 
                UserId = userId, 
                ChatId = chatId1,
                JoinedAt = originalJoinDate
            };
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Chats.AddRange(chat1, chat2);
                context.ChatUsers.Add(chatUser);
                context.SaveChanges();
            }
            
            var newJoinDate = DateTime.UtcNow;

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                
                var existingChatUser = context.ChatUsers.Find(chatUserId);
                existingChatUser.ChatId = chatId2;
                existingChatUser.JoinedAt = newJoinDate;
                
                var result = repository.UpdateChatUser(existingChatUser);

                Assert.NotNull(result);
                Assert.Equal(chatUserId, result.Id);
                Assert.Equal(chatId2, result.ChatId);
                
                context.SaveChanges();
            }
            
            using (var context = new CoursesPlatformContext(options))
            {
                var savedChatUser = context.ChatUsers.Single();
                Assert.Equal(chatUserId, savedChatUser.Id);
                Assert.Equal(chatId2, savedChatUser.ChatId);
                Assert.NotEqual(originalJoinDate, savedChatUser.JoinedAt);
            }
        }
        
        [Fact]
        public void UpdateChatUser_NonExistingChatUser_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            var nonExistingId = Guid.NewGuid();
            
            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                
                var nonExistingChatUser = new ChatUser
                {
                    Id = nonExistingId,
                    UserId = Guid.NewGuid(),
                    ChatId = Guid.NewGuid(),
                    JoinedAt = DateTime.UtcNow
                };
                
                var result = repository.UpdateChatUser(nonExistingChatUser);

                Assert.Null(result);
            }
        }

        [Fact]
        public void DeleteChatUser_ExistingChatUser_RemovesChatUserAndReturnsChatUser()
        {
            var options = CreateNewContextOptions();
            
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            var chatUserId = Guid.NewGuid();
            
            var user = new User { Id = userId, FirstName = "Test", LastName = "User", Email = "test@example.com" };
            var chat = new Chat { Id = chatId, Name = "Test Chat" };
            
            var chatUser = new ChatUser 
            { 
                Id = chatUserId, 
                UserId = userId, 
                ChatId = chatId,
                JoinedAt = DateTime.UtcNow.AddDays(-2)
            };
            
            using (var context = new CoursesPlatformContext(options))
            {
                context.Users.Add(user);
                context.Chats.Add(chat);
                context.SaveChanges();
                
                context.ChatUsers.Add(chatUser);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                var result = repository.DeleteChatUser(chatUserId);

                Assert.NotNull(result);
                Assert.Equal(chatUserId, result.Id);
                Assert.Equal(userId, result.UserId);
                Assert.Equal(chatId, result.ChatId);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.ChatUsers);
            }
        }

        [Fact]
        public void DeleteChatUser_NonExistingChatUser_ReturnsNull()
        {
            var options = CreateNewContextOptions();

            using (var context = new CoursesPlatformContext(options))
            {
                var repository = new ChatUserRepository(context);
                var result = repository.DeleteChatUser(Guid.NewGuid());

                Assert.Null(result);
            }
        }
    }
}
