using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(Guid reviewId);
        Task<List<Review>> GetReviewsByCourseIdAsync(Guid courseId);
        Task<List<Review>> GetReviewsByUserIdAsync(Guid userId);
        Task<Review> AddReviewAsync(Review review);
        Task<Review?> UpdateReviewAsync(Review review);
        Task<Review?> DeleteReviewAsync(Guid reviewId);
        Task<double?> GetAverageRatingForCourseAsync(Guid courseId);
        Task DeleteReviewsAsync(IEnumerable<Guid> reviewIds);
    }
}
