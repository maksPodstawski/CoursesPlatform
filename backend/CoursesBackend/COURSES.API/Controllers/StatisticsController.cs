using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly IReviewService _reviewService;

        public StatisticsController(IUserService userService, ICourseService courseService, IReviewService reviewService)
        {
            _userService = userService;
            _courseService = courseService;
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatistics()
        {
            var usersCount = (await _userService.GetAllUsersAsync()).Count;
            var coursesCount = (await _courseService.GetAllCoursesAsync()).Count;
            var reviewsCount = (await _reviewService.GetAllReviewsAsync()).Count;

            return Ok(new
            {
                usersCount,
                coursesCount,
                reviewsCount
            });
        }
    }
}
