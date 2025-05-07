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
        IQueryable<Chat> GetChats();
        Chat? GetChatById(Guid chatId);
        Chat AddChat(Chat chat);
        Chat? UpdateChat(Chat chat);
        Chat? DeleteChat(Guid chatId);
    }
}
