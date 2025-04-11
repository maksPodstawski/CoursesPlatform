using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class ProgressRepository : IProgressRepository
{
    private readonly CoursesPlatformContext _context;

    public ProgressRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Progress>> GetProgressesAsync()
    {
        return await _context.Progresses.ToListAsync();
    }

    public async Task<Progress?> GetProgressByIdAsync(Guid progressId)
    {
        return await _context.Progresses.FindAsync(progressId);
    }

    public async Task AddProgressAsync(Progress progress)
    {
        await _context.Progresses.AddAsync(progress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProgressAsync(Progress progress)
    {
        _context.Progresses.Update(progress);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProgressAsync(Guid progressId)
    {
        var progress = await GetProgressByIdAsync(progressId);
        if (progress != null)
        {
            _context.Progresses.Remove(progress);
            await _context.SaveChangesAsync();
        }
    }
}
