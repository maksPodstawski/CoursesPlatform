using IDAL;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ChatUser>> GetAllAsync()
        {
            return await _chatUserRepository.GetChatUsers().ToListAsync();
        }

        public async Task<ChatUser?> GetByIdAsync(Guid chatUserId)
        {
            return await Task.FromResult(_chatUserRepository.GetChatUserById(chatUserId));
        }

        public async Task<ChatUser> AddUserToChatAsync(Guid chatId, Guid userId)
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
                _chatUserRepository.AddChatUser(chatUser);
                return chatUser;
            }
            return null;
        }

        public async Task<ChatUser?> RemoveUserFromChatAsync(Guid chatId, Guid userId)
        {
            var chatUsers =  _chatUserRepository.GetChatUsers();
            var target = await Task.FromResult(chatUsers.FirstOrDefault(cu => cu.ChatId == chatId && cu.UserId == userId));

            if (target != null)
            {
                return await Task.FromResult(_chatUserRepository.DeleteChatUser(target.Id));
            }
            return null;
        }

        public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
        {
            var chatUsers =  _chatUserRepository.GetChatUsers();
            return await Task.FromResult(chatUsers.Any(cu => cu.ChatId == chatId && cu.UserId == userId));
        }
    }
}
