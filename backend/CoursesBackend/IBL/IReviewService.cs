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
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(Guid reviewId);
        Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(Guid courseId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Guid reviewId);
    }
}
