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

    public async Task<IEnumerable<Review>> GetReviewsAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(Guid reviewId)
    {
        return await _context.Reviews.FindAsync(reviewId);
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(Guid reviewId)
    {
        var review = await GetReviewByIdAsync(reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
