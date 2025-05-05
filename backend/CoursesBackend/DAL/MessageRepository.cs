using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class MessageRepository : IMessageRepository
{
    private readonly CoursesPlatformContext _context;

    public MessageRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public IQueryable<Message> GetMessages()
    {
        return _context.Messages;
    }

    public Message? GetMessageById(Guid messageId)
    {
        return _context.Messages.FirstOrDefault(m => m.Id == messageId);
    }

    public Message AddMessage(Message message)
    {
        _context.Messages.Add(message);
        _context.SaveChanges();
        return message;
    }

    public Message? UpdateMessage(Message message)
    {
        var existing = _context.Messages.FirstOrDefault(m => m.Id == message.Id);
        if (existing == null)
            return null;

        _context.Messages.Update(message);
        _context.SaveChanges();
        return message;
    }

    public Message? DeleteMessage(Guid messageId)
    {
        var message = _context.Messages.FirstOrDefault(m => m.Id == messageId);
        if (message == null)
            return null;

        _context.Messages.Remove(message);
        _context.SaveChanges();
        return message;
    }
}
