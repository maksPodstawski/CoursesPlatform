using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ChatUserService
    {
        private readonly IChatUserRepository _chatUserRepository;

        public ChatUserService(IChatUserRepository chatUserRepository)
        {
            _chatUserRepository = chatUserRepository;
        }

        public async Task<IEnumerable<ChatUser>> GetAllAsync()
        {
            return await _chatUserRepository.GetChatUsersAsync();
        }

        public async Task<ChatUser?> GetByIdAsync(Guid chatUserId)
        {
            return await _chatUserRepository.GetChatUserByIdAsync(chatUserId);
        }

        public async Task AddUserToChatAsync(Guid chatId, Guid userId)
        {
            var isInChat = await IsUserInChatAsync(chatId, userId);
            if (!isInChat)
            {
                var chatUser = new ChatUser
                {
                    Id = Guid.NewGuid(),
                    ChatId = chatId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                };

                await _chatUserRepository.AddChatUserAsync(chatUser);
            }
        }

        public async Task RemoveUserFromChatAsync(Guid chatId, Guid userId)
        {
            var chatUsers = await _chatUserRepository.GetChatUsersAsync();
            var target = chatUsers.FirstOrDefault(cu => cu.ChatId == chatId && cu.UserId == userId);

            if (target != null)
            {
                await _chatUserRepository.DeleteChatUserAsync(target.Id);
            }
        }

        public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
        {
            var chatUsers = await _chatUserRepository.GetChatUsersAsync();
            return chatUsers.Any(cu => cu.ChatId == chatId && cu.UserId == userId);
        }
    }
}
