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

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _context.Messages.ToListAsync();
    }

    public async Task<Message?> GetByIdAsync(Guid id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task AddAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var message = await GetByIdAsync(id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
}