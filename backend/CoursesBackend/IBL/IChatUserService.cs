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
        Task<IEnumerable<ChatUser>> GetAllAsync();
        Task<ChatUser?> GetByIdAsync(Guid chatUserId);
        Task AddUserToChatAsync(Guid chatId, Guid userId);
        Task RemoveUserFromChatAsync(Guid chatId, Guid userId);
        Task<bool> IsUserInChatAsync(Guid chatId, Guid userId);
    }
}
