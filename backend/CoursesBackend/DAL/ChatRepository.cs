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

    public async Task<IEnumerable<Chat>> GetChatsAsync()
    {
        return await _context.Chats.ToListAsync();
    }

    public async Task<Chat?> GetChatByIdAsync(Guid chatId)
    {
        return await _context.Chats.FindAsync(chatId);
    }

    public async Task AddChatAsync(Chat chat)
    {
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateChatAsync(Chat chat)
    {
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteChatAsync(Guid chatId)
    {
        var chat = await GetChatByIdAsync(chatId);
        if (chat != null)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetUsersInChatAsync(Guid chatId)
    {
        return await _context.ChatUsers
            .Where(cu => cu.ChatId == chatId)
            .Select(cu => cu.User)
            .ToListAsync();
    }

    public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId)
    {
        return await _context.ChatUsers
            .AnyAsync(cu => cu.ChatId == chatId && cu.UserId == userId);
    }

    public async Task<bool> ChatExistsAsync(Guid chatId)
    {
        return await _context.Chats.AnyAsync(c => c.Id == chatId);
    }
}