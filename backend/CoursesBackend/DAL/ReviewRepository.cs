using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;

namespace DAL;

public class ReviewRepository : IReviewRepository
{
    private readonly CoursesPlatformContext _context;

    public ReviewRepository(CoursesPlatformContext context)
    {
        _context = context;
    }

    public IQueryable<Review> GetReviews()
    {
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course);
    }

    public Review? GetReviewById(Guid reviewId)
    {
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefault(r => r.Id == reviewId);
    }

    public Review AddReview(Review review)
    {
        _context.Reviews.Add(review);
        _context.SaveChanges();
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefault(r => r.Id == review.Id)!;
    }

    public Review? UpdateReview(Review review)
    {
        var existing = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefault(r => r.Id == review.Id);
        if (existing == null)
            return null;

        _context.Reviews.Update(review);
        _context.SaveChanges();
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefault(r => r.Id == review.Id);
    }

    public Review? DeleteReview(Guid reviewId)
    {
        var review = _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Course)
            .FirstOrDefault(r => r.Id == reviewId);
        if (review == null)
            return null;

        _context.Reviews.Remove(review);
        _context.SaveChanges();
        return review;
    }
}
