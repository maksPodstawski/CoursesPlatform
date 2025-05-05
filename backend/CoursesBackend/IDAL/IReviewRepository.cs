using Model;

namespace IDAL;

public interface IReviewRepository
{
    IQueryable<Review> GetReviews();
    Review? GetReviewById(Guid reviewId);
    Review AddReview(Review review);
    Review? UpdateReview(Review review);
    Review? DeleteReview(Guid reviewId);
}
