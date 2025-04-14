using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task AddMessageAsync(Message message);
        Task EditMessageAsync(Guid messageId, string newContent);
        Task DeleteMessageAsync(Guid messageId);
        Task<bool> MessageExistsAsync(Guid messageId);
    }
}
