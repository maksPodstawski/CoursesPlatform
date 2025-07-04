using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLLERS.Tests
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatService> _mockChatService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IChatUserService> _mockChatUserService;
        private readonly Mock<IMessageService> _mockMessageService;
        private readonly Mock<ICreatorService> _mockCreatorService;
        private readonly Mock<ICourseService> _mockCourseService;
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _mockChatService = new Mock<IChatService>();
            _mockUserService = new Mock<IUserService>();
            _mockChatUserService = new Mock<IChatUserService>();
            _mockMessageService = new Mock<IMessageService>();
            _mockCreatorService = new Mock<ICreatorService>();
            _mockCourseService = new Mock<ICourseService>();

            _controller = new ChatController(
                _mockChatService.Object,
                _mockUserService.Object,
                _mockChatUserService.Object,
                _mockMessageService.Object,
                _mockCreatorService.Object,
                _mockCourseService.Object
            );

            var userId = Guid.NewGuid().ToString();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task CreateCourseChat_ValidRequest_ReturnsOk()
        {
     
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var courseId = Guid.NewGuid();
            var createChatDto = new CreateChatDTO { Name = "Test Chat" };

            _mockChatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                .ReturnsAsync((Chat?)null);

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId))
                .ReturnsAsync(new Course { Id = courseId, Name = "Course 1" });

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId, FirstName = "John" });

            _mockCreatorService.Setup(s => s.GetCreatorsByCourseAsync(courseId))
                .ReturnsAsync(new List<Creator>()); // no creators

            _mockChatService.Setup(s => s.AddChatAsync(It.IsAny<Chat>()))
                 .ReturnsAsync((Chat chat) => chat);

            _mockChatUserService.Setup(s => s.AddUserToChatAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                 .ReturnsAsync(new ChatUser());

            var result = await _controller.CreateCourseChat(createChatDto, courseId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CreateChatResponseDTO>(okResult.Value);
            Assert.Equal(createChatDto.Name, response.Name);
        }

        [Fact]
        public async Task CreateCourseChat_ChatAlreadyExists_ReturnsConflict()
        {
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var courseId = Guid.NewGuid();
            var createChatDto = new CreateChatDTO { Name = "Test Chat" };

            _mockChatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                .ReturnsAsync(new Chat());

            var result = await _controller.CreateCourseChat(createChatDto, courseId);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Chat for this user and course already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task CreateCourseChat_InvalidDto_ReturnsBadRequest()
        {
            var result = await _controller.CreateCourseChat(null!, Guid.NewGuid());
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid chat data.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCourseChat_UserNotAuthorized_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            var result = await _controller.CreateCourseChat(new CreateChatDTO { Name = "Chat" }, Guid.NewGuid());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task JoinChat_ValidChat_ReturnsOk()
        {
            var chatId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _mockChatService.Setup(s => s.GetChatByIdAsync(chatId))
                .ReturnsAsync(new Chat { Id = chatId });

            _mockChatUserService.Setup(s => s.AddUserToChatAsync(chatId, userId))
                .ReturnsAsync(new ChatUser { ChatId = chatId, UserId = userId });

            var result = await _controller.JoinChat(chatId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<MessageResponseDTO>(ok.Value);
            Assert.Equal("Joined chat successfully.", value.Message);
        }

        [Fact]
        public async Task JoinChat_ChatNotFound_ReturnsNotFound()
        {
            var chatId = Guid.NewGuid();

            _mockChatService.Setup(s => s.GetChatByIdAsync(chatId))
                .ReturnsAsync((Chat?)null);

            var result = await _controller.JoinChat(chatId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Chat not found.", notFound.Value);
        }

        [Fact]
        public async Task JoinChat_Unauthorized_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // no user

            var result = await _controller.JoinChat(Guid.NewGuid());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetMyChats_ReturnsListOfChats()
        {
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid(), Name = "Chat1", CourseId = Guid.NewGuid() }
            };

            _mockChatUserService.Setup(s => s.GetChatsOfUser(userId))
                .ReturnsAsync(chats);

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Course { Id = id, Name = "CourseName" });

            var result = await _controller.GetMyChats();

            var ok = Assert.IsType<OkObjectResult>(result);
            var chatList = Assert.IsAssignableFrom<List<CreateChatResponseDTO>>(ok.Value);
            Assert.Single(chatList);
            Assert.Equal("Chat1", chatList[0].Name);
            Assert.Equal("CourseName", chatList[0].CourseName);
        }

        [Fact]
        public async Task GetMyChats_Unauthorized_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // no user

            var result = await _controller.GetMyChats();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetMessages_UserNotInChat_ReturnsForbid()
        {
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var chatId = Guid.NewGuid();

            _mockChatUserService.Setup(s => s.IsUserInChatAsync(chatId, userId))
                .ReturnsAsync(false);

            var result = await _controller.GetMessages(chatId);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetMessages_ReturnsMessages()
        {
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var chatId = Guid.NewGuid();

            _mockChatUserService.Setup(s => s.IsUserInChatAsync(chatId, userId))
                .ReturnsAsync(true);

            var messages = new List<Message>
            {
                new Message { Id = Guid.NewGuid(), ChatId = chatId, Content = "Hello", CreatedAt = DateTime.UtcNow, Author = new User { FirstName = "Alice" }, AuthorId = Guid.NewGuid() }
            };

            _mockMessageService.Setup(s => s.GetMessagesByChatIdAsync(chatId, 50))
                .ReturnsAsync(messages);

            var result = await _controller.GetMessages(chatId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returnedMessages = Assert.IsAssignableFrom<List<MessageDTO>>(ok.Value);
            Assert.Single(returnedMessages);
            Assert.Equal("Hello", returnedMessages[0].Content);
            Assert.Equal("Alice", returnedMessages[0].AuthorName);
        }

        [Fact]
        public async Task GetChatByCourse_ExistingChat_ReturnsChat()
        {
            var userId = Guid.Parse(_controller.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var courseId = Guid.NewGuid();
            var chat = new Chat { Id = Guid.NewGuid(), Name = "Chat1", ChatAuthorId = userId, CourseId = courseId };

            _mockChatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                .ReturnsAsync(chat);

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId))
                .ReturnsAsync(new Course { Id = courseId, Name = "Course 1" });

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId, FirstName = "John" });

            var result = await _controller.GetChatByCourse(courseId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CreateChatResponseDTO>(ok.Value);
            Assert.Equal("Chat1", response.Name);
        }

        [Fact]
        public async Task GetChatByCourse_NotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            _mockChatService.Setup(s => s.GetChatByAuthorAndCourseAsync(userId, courseId))
                .ReturnsAsync((Chat?)null);

            _mockCourseService.Setup(s => s.GetCourseByIdAsync(courseId))
                .ReturnsAsync((Course?)null); 

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId, FirstName = "John" });
            _mockCreatorService.Setup(s => s.GetCreatorsByCourseAsync(courseId))
                .ReturnsAsync(new List<Creator>());

            var result = await _controller.GetChatByCourse(courseId);


            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Course not found.", notFound.Value);
        }

        [Fact]
        public async Task GetChatByCourse_Unauthorized_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // no user

            var result = await _controller.GetChatByCourse(Guid.NewGuid());

            Assert.IsType<UnauthorizedResult>(result);
        }
    }
   
}
