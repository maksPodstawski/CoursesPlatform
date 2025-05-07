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
        Message? GetMessageById(Guid messageId);
        Message AddMessage(Message message);
        Message? UpdateMessage(Message message);
        Message? DeleteMessage(Guid messageId);
    }
}
