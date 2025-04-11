using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL;

public class ReviewRepository : IReviewRepository
{
    private readonly CoursesPlatformContext _context;

    public ReviewRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}