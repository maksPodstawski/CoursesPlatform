using System.Security.Claims;
using System.Xml.Linq;
using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DTO;

namespace COURSES.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IChatUserService _chatUserService;
        private readonly IMessageService _messageService;
        private readonly ICreatorService _creatorService;
        private readonly ICourseService _courseService;

        public ChatController(IChatService chatService, IUserService userService, IChatUserService chatUserService, IMessageService messageService, ICreatorService creatorService, ICourseService courseService)
        {
            _chatService = chatService;
            _userService = userService;
            _chatUserService = chatUserService;
            _messageService = messageService;
            _creatorService = creatorService;
            _courseService = courseService;
        }

        /* [HttpPost("create")]
         public async Task<IActionResult> CreateChat([FromBody] CreateChatDTO createChatDto)
         {
             var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             if (createChatDto == null)
             {
                 return BadRequest("Invalid chat data.");
             }
             if (string.IsNullOrEmpty(userId))
             {
                 return Unauthorized();
             }

             var chat = new Chat
             {
                 Id = Guid.NewGuid(),
                 Name = createChatDto.Name,
             };

             await _chatService.AddChatAsync(chat);
             await _chatUserService.AddUserToChatAsync(chat.Id, Guid.Parse(userId));
             return Ok(new CreateChatResponseDTO { Id = chat.Id, Name = chat.Name });
         }*/

        [HttpPost("/course/create")]
        public async Task<IActionResult> CreateCourseChat([FromBody] CreateChatDTO createChatDto, [FromQuery] Guid courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (createChatDto == null)
                return BadRequest("Invalid chat data.");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userGuid = Guid.Parse(userId);

            var existingChat = await _chatService.GetChatByAuthorAndCourseAsync(userGuid, courseId);
            if (existingChat != null)
            {
                return Conflict("Chat for this user and course already exists.");
            }

            Course course = await _courseService.GetCourseByIdAsync(courseId);
            User user = await _userService.GetUserByIdAsync(userGuid);
            if (course == null)
            {
                return NotFound("Course not found.");
            }
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = $"{course.Name} - {user.ToString()}",
                ChatAuthorId = userGuid,
                CourseId = courseId
            };

            var creators = await _creatorService.GetCreatorsByCourseAsync(courseId);

            await _chatService.AddChatAsync(chat);
            await _chatUserService.AddUserToChatAsync(chat.Id, userGuid);

            foreach (var creator in creators)
            {
                await _chatUserService.AddUserToChatAsync(chat.Id, creator.UserId);
            }

            return Ok(new CreateChatResponseDTO { Id = chat.Id, Name = chat.Name });
        }


        [HttpPost("{chatId}/join")]
        public async Task<IActionResult> JoinChat(Guid chatId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var chat = await _chatService.GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return NotFound("Chat not found.");
            }
            var user = _userService.GetUserByIdAsync(Guid.Parse(userId));
            await _chatUserService.AddUserToChatAsync(chatId, Guid.Parse(userId));
            return Ok(new { Message = "Joined chat successfully." });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var chats = await _chatUserService.GetChatsOfUser(Guid.Parse(userId));

            var result = new List<CreateChatResponseDTO>();
            foreach (var chat in chats)
            {
                var course = await _courseService.GetCourseByIdAsync(chat.CourseId);
                result.Add(new CreateChatResponseDTO
                {
                    Id = chat.Id,
                    Name = chat.Name,
                    CourseId = chat.CourseId,
                    CourseName = course?.Name ?? string.Empty
                });
            }

            return Ok(result);
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(Guid chatId, [FromQuery] int count = 50)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            bool isMember = _chatUserService.IsUserInChatAsync(chatId, Guid.Parse(userId)).Result;
            if (!isMember) return Forbid();

            var messages = await _messageService.GetMessagesByChatIdAsync(chatId);
            var lastMessages = messages.OrderByDescending(m => m.CreatedAt).Take(count).OrderBy(m => m.CreatedAt);

            var result = lastMessages.Select(m => new MessageDTO
            {
                Id = m.Id,
                ChatId = m.ChatId,
                AuthorId = m.AuthorId,
                AuthorName = m.Author?.FirstName ?? "",
                Content = m.Content,
                CreatedAt = m.CreatedAt
            }).ToList();

            return Ok(result);
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetChatByCourse(Guid courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var userGuid = Guid.Parse(userId);
            var chat = await _chatService.GetChatByAuthorAndCourseAsync(userGuid, courseId);

            Course course = await _courseService.GetCourseByIdAsync(courseId);
            User user = await _userService.GetUserByIdAsync(userGuid);
            
            if (chat == null)
            {
                var newChat = new Chat
                {
                    Id = Guid.NewGuid(),
                    Name = $"{course?.Name} - {user?.ToString()}",
                    ChatAuthorId = userGuid,
                    CourseId = courseId
                };

                var creators = await _creatorService.GetCreatorsByCourseAsync(courseId);

                await _chatService.AddChatAsync(newChat);
                await _chatUserService.AddUserToChatAsync(newChat.Id, userGuid);

                foreach (var creator in creators)
                {
                    await _chatUserService.AddUserToChatAsync(newChat.Id, creator.UserId);
                }

                return Ok(new CreateChatResponseDTO { Id = newChat.Id, Name = newChat.Name });
            }

            return Ok(new CreateChatResponseDTO { Id = chat.Id, Name = chat.Name });
        }
    }
}
