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

    public async Task<IEnumerable<Message>> GetMessagesAsync()
    {
        return await _context.Messages.ToListAsync()
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages.FindAsync(messageId);
    }

    public async Task AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMessageAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await GetMessageByIdAsync(messageId);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
    {
        return await _context.Messages
            .Where(m => m.ChatId == chatId && !m.IsDeleted)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}
