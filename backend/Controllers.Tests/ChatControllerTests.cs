using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Controllers.Tests
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> _chatService;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IChatUserService> _chatUserService;
        private readonly Mock<IMessageService> _messageService;
        private readonly Mock<ICreatorService> _creatorService;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _chatService = new Mock<IChatService>();
            _userService = new Mock<IUserService>();
            _chatUserService = new Mock<IChatUserService>();
            _messageService = new Mock<IMessageService>();
            _creatorService = new Mock<ICreatorService>();

            _controller = new ChatController(
                _chatService.Object,
                _userService.Object,
                _chatUserService.Object,
                _messageService.Object,
                _creatorService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private void SetUser(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        [Fact]
        public async Task CreateCourseChat_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId);

            var dto = new CreateChatDTO { Name = "New Chat" };
            _chatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                        .ReturnsAsync((Chat)null);
            _creatorService.Setup(c => c.GetCreatorsByCourseAsync(courseId))
                           .ReturnsAsync(new List<Creator>());

            var result = await _controller.CreateCourseChat(dto, courseId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var chatResponse = Assert.IsType<CreateChatResponseDTO>(ok.Value);
            Assert.Equal(dto.Name, chatResponse.Name);
        }

        [Fact]
        public async Task CreateCourseChat_ChatExists_ReturnsConflict()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId);

            _chatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                        .ReturnsAsync(new Chat());

            var result = await _controller.CreateCourseChat(new CreateChatDTO { Name = "x" }, courseId);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Chat for this user and course already exists.", conflict.Value);
        }

        [Fact]
        public async Task JoinChat_Valid_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            SetUser(userId);

            _chatService.Setup(s => s.GetChatByIdAsync(chatId)).ReturnsAsync(new Chat());

            var result = await _controller.JoinChat(chatId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = JObject.FromObject(ok.Value);
            Assert.Equal("Joined chat successfully.", (string)response["Message"]);
        }

        [Fact]
        public async Task JoinChat_ChatNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            SetUser(userId);

            _chatService.Setup(s => s.GetChatByIdAsync(chatId)).ReturnsAsync((Chat)null);

            var result = await _controller.JoinChat(chatId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Chat not found.", notFound.Value);
        }

        [Fact]
        public async Task GetMyChats_ValidUser_ReturnsChats()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId);

            var chatDto1 = new CreateChatDTO { Name = "Chat A" };
            var chatDto2 = new CreateChatDTO { Name = "Chat B" };

            var chats = new List<Chat>
            {
               Chat.ChatFromDTO(chatDto1, userId, courseId),
               Chat.ChatFromDTO(chatDto2, userId, courseId)
            };
            _chatUserService.Setup(s => s.GetChatsOfUser(userId)).ReturnsAsync(chats);

            var result = await _controller.GetMyChats();

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<List<CreateChatResponseDTO>>(ok.Value);
            Assert.Equal(2, returned.Count);
            Assert.Contains(returned, c => c.Name == "Chat A");
            Assert.Contains(returned, c => c.Name == "Chat B");
        }

        [Fact]
        public async Task GetMessages_UserNotInChat_ReturnsForbid()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            SetUser(userId);

            _chatUserService.Setup(s => s.IsUserInChatAsync(chatId, userId)).ReturnsAsync(false);

            var result = await _controller.GetMessages(chatId);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetMessages_Valid_ReturnsMessages()
        {
            var userId = Guid.NewGuid();
            var chatId = Guid.NewGuid();
            SetUser(userId);

            var messages = new List<Message>
            {
                new Message
                {
                    Id = Guid.NewGuid(),
                    ChatId = chatId,
                    Content = "Hello",
                    AuthorId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Author = new User { FirstName = "Test" }
                }
            };

            _chatUserService.Setup(s => s.IsUserInChatAsync(chatId, userId)).ReturnsAsync(true);
            _messageService.Setup(s => s.GetMessagesByChatIdAsync(chatId, 50)).ReturnsAsync(messages);

            var result = await _controller.GetMessages(chatId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IEnumerable<MessageDTO>>(ok.Value);

            Assert.Single(returned);
            Assert.Equal(messages[0].Id, returned.First().Id);
            Assert.Equal(messages[0].Content, returned.First().Content);
            Assert.Equal(messages[0].Author?.FirstName, returned.First().AuthorName);
        }

        [Fact]
        public async Task GetChatByCourse_ChatDoesNotExist_CreatesAndReturnsChat()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId);

            _chatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId)).ReturnsAsync((Chat)null);
            _creatorService.Setup(c => c.GetCreatorsByCourseAsync(courseId)).ReturnsAsync(new List<Creator>());

            var result = await _controller.GetChatByCourse(courseId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CreateChatResponseDTO>(ok.Value);
            Assert.Equal($"Course Chat - {courseId}", dto.Name);
        }

        [Fact]
        public async Task GetChatByCourse_ExistingChat_ReturnsChat()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId);

            var chat = new Chat { Id = Guid.NewGuid(), Name = "Existing Chat" };
            _chatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId)).ReturnsAsync(chat);

            var result = await _controller.GetChatByCourse(courseId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CreateChatResponseDTO>(ok.Value);
            Assert.Equal(chat.Id, dto.Id);
        }

    }
    public class MessageResponse
    {
        public string Message { get; set; }
    }
}
