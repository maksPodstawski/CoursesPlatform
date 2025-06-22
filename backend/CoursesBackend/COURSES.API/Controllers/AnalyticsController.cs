using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IBL;
using Model.DTO;
using System.Security.Claims;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("my-analytics")]
        public async Task<ActionResult<CreatorAnalyticsDTO>> GetMyAnalytics([FromQuery] int? year)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var analytics = await _analyticsService.GetMyAnalyticsAsync(Guid.Parse(userId), year ?? DateTime.UtcNow.Year);
                return Ok(analytics);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving analytics data");
            }
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<ActionResult<CreatorAnalyticsDTO>> GetCreatorAnalytics(Guid creatorId, [FromQuery] int? year)
        {
            try
            {
                var analytics = await _analyticsService.GetCreatorAnalyticsAsync(creatorId, year ?? DateTime.UtcNow.Year);
                return Ok(analytics);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving analytics data");
            }
        }

        [HttpGet("courses")]
        public async Task<ActionResult<List<CourseAnalyticsDTO>>> GetMyCourseAnalytics()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var creator = await _analyticsService.GetMyAnalyticsAsync(Guid.Parse(userId));
                if (creator.CreatorId == Guid.Empty)
                {
                    return Ok(new List<CourseAnalyticsDTO>());
                }
                var courseAnalytics = await _analyticsService.GetCourseAnalyticsAsync(creator.CreatorId);
                return Ok(courseAnalytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving course analytics");
            }
        }

        [HttpGet("monthly-revenue/{year}")]
        public async Task<ActionResult<List<MonthlyRevenueDTO>>> GetMonthlyRevenue(int year)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var creatorResult = await _analyticsService.GetMyAnalyticsAsync(Guid.Parse(userId));
                if (creatorResult.CreatorId == Guid.Empty)
                {
                    return Ok(new List<MonthlyRevenueDTO>());
                }
                var monthlyRevenue = await _analyticsService.GetMonthlyRevenueAsync(creatorResult.CreatorId, year);
                return Ok(monthlyRevenue);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving monthly revenue data");
            }
        }

        [HttpGet("top-performing-courses")]
        public async Task<ActionResult<List<CoursePerformanceDTO>>> GetTopPerformingCourses([FromQuery] int limit = 5)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var creatorResult = await _analyticsService.GetMyAnalyticsAsync(Guid.Parse(userId));
                if (creatorResult.CreatorId == Guid.Empty)
                {
                    return Ok(new List<CoursePerformanceDTO>());
                }
                var topCourses = await _analyticsService.GetTopPerformingCoursesAsync(creatorResult.CreatorId, limit);
                return Ok(topCourses);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving top performing courses");
            }
        }
    }
} 