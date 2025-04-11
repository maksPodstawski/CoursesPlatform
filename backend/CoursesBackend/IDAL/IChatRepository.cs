using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetChatsAsync();
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task AddChatAsync(Chat chat);
        Task UpdateChatAsync(Chat chat);
        Task DeleteChatAsync(Guid chatId);
    }
}
