using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IBL;
using Model;
using Model.DTO;
using System.Security.Claims;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICourseService _courseService;

        public ReviewsController(IReviewService reviewService, ICourseService courseService)
        {
            _reviewService = reviewService;
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewResponseDTO>>> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews.Select(ReviewResponseDTO.FromReview));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponseDTO>> GetReviewById(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(ReviewResponseDTO.FromReview(review));
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ReviewResponseDTO>>> GetReviewsByCourse(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
            return Ok(reviews.Select(ReviewResponseDTO.FromReview));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewResponseDTO>> CreateReview([FromBody] CreateReviewDTO createReviewDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var course = await _courseService.GetCourseByIdAsync(createReviewDto.CourseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var existingReviews = await _reviewService.GetReviewsByUserIdAsync(Guid.Parse(userId));
            if (existingReviews.Any(r => r.CourseId == createReviewDto.CourseId))
            {
                return BadRequest("You have already reviewed this course");
            }

            var review = new Review
            {
                Rating = createReviewDto.Rating,
                Comment = createReviewDto.Comment,
                UserId = Guid.Parse(userId),
                CourseId = createReviewDto.CourseId,
                CreatedAt = DateTime.UtcNow
            };

            var createdReview = await _reviewService.AddReviewAsync(review);
            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, ReviewResponseDTO.FromReview(createdReview));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResponseDTO>> UpdateReview(Guid id, [FromBody] UpdateReviewDTO updateReviewDto)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || review.UserId != Guid.Parse(userId))
            {
                return Forbid();
            }

            review.Rating = updateReviewDto.Rating;
            review.Comment = updateReviewDto.Comment;

            var updatedReview = await _reviewService.UpdateReviewAsync(review);
            if (updatedReview == null)
            {
                return BadRequest("Failed to update review");
            }

            return Ok(ReviewResponseDTO.FromReview(updatedReview));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || review.UserId != Guid.Parse(userId))
            {
                return Forbid();
            }

            var deletedReview = await _reviewService.DeleteReviewAsync(id);
            if (deletedReview == null)
            {
                return BadRequest("Failed to delete review");
            }

            return NoContent();
        }

        [HttpGet("course/{courseId}/average-rating")]
        public async Task<ActionResult<double?>> GetAverageRating(Guid courseId)
        {
            var avg = await _reviewService.GetAverageRatingForCourseAsync(courseId);
            return Ok(avg);
        }
    }
} 