using System.Security.Claims;
using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Model;

namespace COURSES.API.Hubs
{
    [Authorize]
    public class ChatHub: Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly IChatUserService _chatUserService;
        private readonly IMessageService _messageService;
        public ChatHub(IChatService chatService, IUserService userService, IChatUserService chatUserService, IMessageService messageService)
        {
            _chatService = chatService;
            _userService = userService;
            _chatUserService = chatUserService;
            _messageService = messageService;
        }
        private Guid? GetUserId()
        {
            var userIdStr = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdStr, out var guid) ? guid : null;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId == null) return;

            var chats = await _chatUserService.GetChatsOfUser(userId.Value);

            foreach (var chat in chats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
            }

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(Guid chatId, string content)
        {
            var userId = GetUserId();
            if (userId == null || string.IsNullOrWhiteSpace(content)) return;

            bool isMember = await _chatUserService.IsUserInChatAsync(chatId, userId.Value);
            if (!isMember) return;

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null) return;

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                AuthorId = userId.Value,
                Author = user,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _messageService.AddMessageAsync(message);

            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", new
            {
                ChatId = message.ChatId,
                AuthorId = message.AuthorId,
                AuthorName = message.Author.FirstName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            });
        }

        public async Task JoinChat(Guid chatId)
        {
            var userId = GetUserId();
            if (userId == null) return;

            bool isMember = await _chatUserService.IsUserInChatAsync(chatId, userId.Value);
            if (!isMember)
            {
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task LeaveChat(Guid chatId)
        {
            var userId = GetUserId();
            if (userId == null) return;

            await _chatUserService.RemoveUserFromChatAsync(userId.Value, chatId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
        }
    }
}
