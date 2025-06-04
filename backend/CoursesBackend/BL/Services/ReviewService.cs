using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<List<Review>> GetAllReviewsAsync()
        {
            return await _reviewRepository.GetReviews().ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(Guid reviewId)
        {
            return await Task.FromResult(_reviewRepository.GetReviewById(reviewId));
        }

        public async Task<List<Review>> GetReviewsByCourseIdAsync(Guid courseId)
        {
            return await _reviewRepository.GetReviews()
               .Where(r => r.CourseId == courseId)
               .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(Guid userId)
        {
            return await _reviewRepository.GetReviews()
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            review.Id = Guid.NewGuid();
            //W opini trzeba dodac jeszcze date nowa!!!
            return await Task.FromResult(_reviewRepository.AddReview(review));
            
        }

        public async Task<Review?> UpdateReviewAsync(Review review)
        {
            return await Task.FromResult(_reviewRepository.UpdateReview(review));
        }

        public async Task<Review?> DeleteReviewAsync(Guid reviewId)
        {
            return await Task.FromResult(_reviewRepository.DeleteReview(reviewId));
        }

        public async Task<double?> GetAverageRatingForCourseAsync(Guid courseId)
        {
            var reviews = await _reviewRepository.GetReviews()
                .Where(r => r.CourseId == courseId)
                .ToListAsync();

            if (reviews == null || !reviews.Any())
                return null;

            return reviews.Average(r => r.Rating);
        }


        public async Task DeleteReviewsAsync(IEnumerable<Guid> reviewIds)
        {
            foreach (var id in reviewIds)
            {
                _reviewRepository.DeleteReview(id);
            }

            await Task.CompletedTask;
        }

        public async Task<Review?> GetReviewByUserAndCourseIdAsync(Guid userId, Guid courseId)
        {
                var review = await _reviewRepository.GetReviews()
            .Where(r => r.UserId == userId && r.CourseId == courseId)
            .FirstOrDefaultAsync();

                return review;
        }
    }
}
