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
        IQueryable<ChatUser> GetChatUsers();
        ChatUser? GetChatUserById(Guid chatUserId);
        ChatUser? AddChatUser(ChatUser chatUser);
        ChatUser? UpdateChatUser(ChatUser chatUser);
        ChatUser? DeleteChatUser(Guid chatUserId);
    }
}
