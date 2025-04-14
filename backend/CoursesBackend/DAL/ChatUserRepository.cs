using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class ChatUserRepository : IChatUserRepository
{
    private readonly CoursesPlatformContext _context;

    public ChatUserRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatUser>> GetChatUsersAsync()
    {
        return await _context.ChatUsers.ToListAsync();
    }

    public async Task<ChatUser?> GetChatUserByIdAsync(Guid chatUserId)
    {
        return await _context.ChatUsers.FindAsync(chatUserId);
    }

    public async Task AddChatUserAsync(ChatUser chatUser)
    {
        await _context.ChatUsers.AddAsync(chatUser);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateChatUserAsync(ChatUser chatUser)
    {
        _context.ChatUsers.Update(chatUser);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChatUserAsync(Guid chatUserId)
    {
        var chatUser = await GetChatUserByIdAsync(chatUserId);
        if (chatUser != null)
        {
            _context.ChatUsers.Remove(chatUser);
            await _context.SaveChangesAsync();
        }
    }
   
}
