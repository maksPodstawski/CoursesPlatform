using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IChatUserService
    {
        Task<List<ChatUser>> GetAllAsync();
        Task<ChatUser?> GetByIdAsync(Guid chatUserId);
        Task<ChatUser> AddUserToChatAsync(Guid chatId, Guid userId);
        Task<ChatUser?> RemoveUserFromChatAsync(Guid chatId, Guid userId);
        Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
    }
}
