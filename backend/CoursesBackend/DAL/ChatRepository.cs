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

    public async Task<IEnumerable<Chat>> GetAllAsync()
    {
        return await _context.Chats.ToListAsync();
    }

    public async Task<Chat?> GetByIdAsync(Guid id)
    {
        return await _context.Chats.FindAsync(id);
    }

    public async Task AddAsync(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Chat chat)
    {
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var chat = await GetByIdAsync(id);
        if (chat != null)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
        }
    }
}