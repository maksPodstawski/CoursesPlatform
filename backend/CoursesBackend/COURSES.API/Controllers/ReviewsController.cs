using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Constans;
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
            if (review == null) return NotFound();
            return Ok(ReviewResponseDTO.FromReview(review));
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ReviewResponseDTO>>> GetReviewsByCourse(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound("Course not found");

            var reviews = await _reviewService.GetReviewsByCourseIdAsync(courseId);
            return Ok(reviews.Select(ReviewResponseDTO.FromReview));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewResponseDTO>> CreateReview([FromBody] CreateReviewDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var course = await _courseService.GetCourseByIdAsync(dto.CourseId);
            if (course == null) return NotFound("Course not found");

            var existing = await _reviewService.GetReviewsByUserIdAsync(Guid.Parse(userId));
            if (existing.Any(r => r.CourseId == dto.CourseId)) return BadRequest("Already reviewed");

            var review = Review.FromCreateDTO(dto, Guid.Parse(userId));

            var created = await _reviewService.AddReviewAsync(review);
            return CreatedAtAction(nameof(GetReviewById), new { id = created.Id }, ReviewResponseDTO.FromReview(created));
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewResponseDTO>> UpdateReview(Guid id, [FromBody] UpdateReviewDTO dto)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null) return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || review.UserId != Guid.Parse(userId)) return Forbid();

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            var updated = await _reviewService.UpdateReviewAsync(review);
            if (updated == null) return BadRequest("Failed to update");

            return Ok(ReviewResponseDTO.FromReview(updated));
        }

        [HttpGet("course/{courseId}/rating-summary")]
        public async Task<IActionResult> GetRatingSummary(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound("Course not found");

            var avg = await _reviewService.GetAverageRatingForCourseAsync(courseId);
            var count = await _reviewService.GetReviewsByCourseIdAsync(courseId);

            return Ok(new { averageRating = Math.Round(avg ?? 0, 1), reviewCount = count.Count });
        }

        [Authorize]
        [HttpGet("course/{courseId}/user")]
        public async Task<ActionResult<ReviewResponseDTO>> GetUserReviewForCourse(Guid courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var review = await _reviewService.GetReviewByUserAndCourseIdAsync(Guid.Parse(userId), courseId);
            if (review == null) return NotFound();

            return Ok(ReviewResponseDTO.FromReview(review));
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
    }
}
