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
        IQueryable<Review> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(Guid reviewId);
        IQueryable<Review> GetReviewsByCourseIdAsync(Guid courseId);
        IQueryable<Review> GetReviewsByUserIdAsync(Guid userId);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Guid reviewId);
    }
}
