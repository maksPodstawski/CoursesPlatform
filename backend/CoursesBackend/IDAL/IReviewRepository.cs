using Model;

namespace IDAL;

public interface IReviewRepository
{
    IQueryable<Review> GetReviews();
    Task<Review?> GetReviewByIdAsync(Guid reviewId);
    Task AddReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(Guid reviewId);
}
