using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IMessageRepository
    {
        IQueryable<Message> GetMessages();
        Task<Message?> GetMessageByIdAsync(Guid messageId);
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(Guid messageId);
    }
}
