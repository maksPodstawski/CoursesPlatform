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

    public IQueryable<Progress> GetProgresses()
    {
        return _context.Progresses;
    }

    public Progress? GetProgressById(Guid progressId)
    {
        return _context.Progresses.FirstOrDefault(p => p.Id == progressId);
    }

    public Progress AddProgress(Progress progress)
    {
        _context.Progresses.Add(progress);
        _context.SaveChanges();
        return progress;
    }

    public Progress? UpdateProgress(Progress progress)
    {
        var existing = _context.Progresses.FirstOrDefault(p => p.Id == progress.Id);
        if (existing == null)
            return null;

        _context.Progresses.Update(progress);
        _context.SaveChanges();
        return progress;
    }

    public Progress? DeleteProgress(Guid progressId)
    {
        var progress = _context.Progresses.FirstOrDefault(p => p.Id == progressId);
        if (progress == null)
            return null;

        _context.Progresses.Remove(progress);
        _context.SaveChanges();
        return progress;
    }
}
