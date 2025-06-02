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
        Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, int count = 50);
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task<Message> AddMessageAsync(Message message);
        Task<Message?> EditMessageAsync(Guid messageId, string newContent);
        Task<Message?> DeleteMessageAsync(Guid messageId);
        Task<bool> MessageExistsAsync(Guid messageId);
    }
}
