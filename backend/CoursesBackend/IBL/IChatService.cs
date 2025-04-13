using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IChatService
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();
        Task<Chat?> GetChatByIdAsync(Guid chatId);
        Task AddChatAsync(Chat chat);
        Task<Chat?> DeleteChatAsync(Guid chatId);
        Task<IEnumerable<User>> GetUsersInChatAsync(Guid chatId);
        Task RenameChatAsync(Guid chatId, string chatName);
        Task<bool> ChatExistsAsync(Guid chatId);
    }
}
