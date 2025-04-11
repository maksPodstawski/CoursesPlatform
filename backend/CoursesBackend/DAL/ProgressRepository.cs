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

    public async Task<IEnumerable<Progress>> GetAllAsync()
    {
        return await _context.Progresses.ToListAsync();
    }

    public async Task<Progress?> GetByIdAsync(Guid id)
    {
        return await _context.Progresses.FindAsync(id);
    }

    public async Task AddAsync(Progress progress)
    {
        await _context.Progresses.AddAsync(progress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Progress progress)
    {
        _context.Progresses.Update(progress);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var progress = await GetByIdAsync(id);
        if (progress != null)
        {
            _context.Progresses.Remove(progress);
            await _context.SaveChangesAsync();
        }
    }
}