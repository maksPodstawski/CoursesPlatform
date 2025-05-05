using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;

namespace DAL;

public class ChatUserRepository : IChatUserRepository
{
    private readonly CoursesPlatformContext _context;

    public ChatUserRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public IQueryable<ChatUser> GetChatUsers()
    {
        return _context.ChatUsers;
    }

    public ChatUser? GetChatUserById(Guid chatUserId)
    {
        return _context.ChatUsers.FirstOrDefault(c => c.Id == chatUserId);
    }

    public ChatUser AddChatUser(ChatUser chatUser)
    {
        _context.ChatUsers.Add(chatUser);
        _context.SaveChanges();
        return chatUser;
    }

    public ChatUser? UpdateChatUser(ChatUser chatUser)
    {
        var existing = _context.ChatUsers.FirstOrDefault(c => c.Id == chatUser.Id);
        if (existing == null)
            return null;

        _context.ChatUsers.Update(chatUser);
        _context.SaveChanges();
        return chatUser;
    }

    public ChatUser? DeleteChatUser(Guid chatUserId)
    {
        var chatUser = _context.ChatUsers.FirstOrDefault(c => c.Id == chatUserId);
        if (chatUser == null)
            return null;

        _context.ChatUsers.Remove(chatUser);
        _context.SaveChanges();
        return chatUser;
    }
   
}
