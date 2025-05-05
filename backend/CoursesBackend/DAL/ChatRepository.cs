using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class ChatRepository : IChatRepository
{
    private readonly CoursesPlatformContext _context;

    public ChatRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public IQueryable<Chat> GetChats()
    {
        return _context.Chats;
    }

    public Chat? GetChatById(Guid chatId)
    {
        return _context.Chats.FirstOrDefault(c => c.Id == chatId);
    }

    public Chat AddChat(Chat chat)
    {
         _context.Chats.Add(chat);
         _context.SaveChanges();
        return chat;
    }

    public Chat? UpdateChat(Chat chat)
    {
        var existing = _context.Chats.FirstOrDefault(c => c.Id == chat.Id);
        if (existing == null)
            return null;

        _context.Chats.Update(chat);
        _context.SaveChanges();
        return chat;
    }

    public Chat? DeleteChat(Guid chatId)
    {
        var chat = _context.Chats.FirstOrDefault(c => c.Id == chatId);
        if (chat == null)
            return null;

        _context.Chats.Remove(chat);
        _context.SaveChanges();
        return chat;
    }
}