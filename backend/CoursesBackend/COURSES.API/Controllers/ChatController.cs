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
        public ChatController(IChatService chatService, IUserService userService, IChatUserService chatUserService, IMessageService messageService)
        {
            _chatService = chatService;
            _userService = userService;
            _chatUserService = chatUserService;
            _messageService = messageService;
        }

        [HttpPost("create")]
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

            var result = chats.Select(chat => new CreateChatResponseDTO
            {
                Id = chat.Id,
                Name = chat.Name,
            }).ToList();

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

            return Ok(messages);
        }
    }
}
