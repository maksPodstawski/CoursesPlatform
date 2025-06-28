using Microsoft.AspNetCore.Mvc;

namespace COURSES.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using IBL;
    using Model;
    using Model.DTO;
    using System.Security.Claims;
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ICourseService _courseService;

        public ReviewController(IReviewService reviewService, ICourseService courseService)
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

            var review = Review.FromCreateDTO(createReviewDto, Guid.Parse(userId));
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

            review.UpdateFromDTO(updateReviewDto);

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

        [HttpGet("course/{courseId}/rating-summary")]
        public async Task<IActionResult> GetRatingSummary(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var averageRating = await _reviewService.GetAverageRatingForCourseAsync(courseId);
            var reviewCount = await _reviewService.GetReviewsByCourseIdAsync(courseId);


            return Ok(new
            {
                averageRating = Math.Round(averageRating ?? 0, 1), // zaokrąglone np. 4.3
                reviewCount = reviewCount.Count
            });
        }
        [Authorize]
        [HttpGet("course/{courseId}/user")]
        public async Task<ActionResult<ReviewResponseDTO>> GetUserReviewForCourse(Guid courseId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var review = await _reviewService.GetReviewByUserAndCourseIdAsync(Guid.Parse(userId), courseId);
            if (review == null)
            {
                return NotFound();
            }

            return Ok(ReviewResponseDTO.FromReview(review));
        }
    }
}
