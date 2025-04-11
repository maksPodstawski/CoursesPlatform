using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IChatUserRepository
    {
        Task<IEnumerable<ChatUser>> GetChatUsersAsync();
        Task<ChatUser?> GetChatUserByIdAsync(Guid chatUserId);
        Task AddChatUserAsync(ChatUser chatUser);
        Task UpdateChatUserAsync(ChatUser chatUser);
        Task DeleteChatUserAsync(Guid chatUserId);
    }
}
