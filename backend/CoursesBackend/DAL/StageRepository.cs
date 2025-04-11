using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class StageRepository : IStageRepository
{
    private readonly CoursesPlatformContext _context;

    public StageRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Stage>> GetAllAsync()
    {
        return await _context.Stages.ToListAsync();
    }

    public async Task<Stage?> GetByIdAsync(Guid id)
    {
        return await _context.Stages.FindAsync(id);
    }

    public async Task AddAsync(Stage stage)
    {
        await _context.Stages.AddAsync(stage);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stage stage)
    {
        _context.Stages.Update(stage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var stage = await GetByIdAsync(id);
        if (stage != null)
        {
            _context.Stages.Remove(stage);
            await _context.SaveChangesAsync();
        }
    }
}