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

    public IQueryable<Stage> GetStages()
    {
        return _context.Stages.AsQueryable();
    }

    public async Task<Stage?> GetStageByIdAsync(Guid stageId)
    {
        return await _context.Stages.FindAsync(stageId);
    }

    public async Task AddStageAsync(Stage stage)
    {
        await _context.Stages.AddAsync(stage);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStageAsync(Stage stage)
    {
        _context.Stages.Update(stage);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStageAsync(Guid stageId)
    {
        var stage = await GetStageByIdAsync(stageId);
        if (stage != null)
        {
            _context.Stages.Remove(stage);
            await _context.SaveChangesAsync();
        }
    }
}
