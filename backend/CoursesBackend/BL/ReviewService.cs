using IBL;
using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetReviewsAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(Guid reviewId)
        {
            return await _reviewRepository.GetReviewByIdAsync(reviewId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByCourseIdAsync(Guid courseId)
        {
            var reviews = await _reviewRepository.GetReviewsAsync();
            return reviews.Where(r => r.CourseId == courseId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetReviewsAsync();
            return reviews.Where(r => r.UserId == userId);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _reviewRepository.AddReviewAsync(review);
        }

        public async Task UpdateReviewAsync(Review review)
        {
            await _reviewRepository.UpdateReviewAsync(review);
        }

        public async Task DeleteReviewAsync(Guid reviewId)
        {
            await _reviewRepository.DeleteReviewAsync(reviewId);
        }
    }
}
