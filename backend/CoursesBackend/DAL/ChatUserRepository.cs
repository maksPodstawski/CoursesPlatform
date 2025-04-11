using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class ChatUserRepository :IChatUserRepository
{
    private readonly CoursesPlatformContext _context;

    public ChatUserRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatUser>> GetAllAsync()
    {
        return await _context.ChatUsers.ToListAsync();
    }

    public async Task<ChatUser?> GetByIdAsync(Guid id)
    {
        return await _context.ChatUsers.FindAsync(id);
    }

    public async Task AddAsync(ChatUser chatUser)
    {
        await _context.ChatUsers.AddAsync(chatUser);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ChatUser chatUser)
    {
        _context.ChatUsers.Update(chatUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var chatUser = await GetByIdAsync(id);
        if (chatUser != null)
        {
            _context.ChatUsers.Remove(chatUser);
            await _context.SaveChangesAsync();
        }
    }
}