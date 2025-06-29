using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IBL;
using Model;
using Model.DTO;
using Model.Constans;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/courses/purchases")]
    public class CoursePurchaseController : ControllerBase
    {
        private readonly IPurchasedCoursesService _purchasedCoursesService;
        private readonly ICourseService _courseService;

        public CoursePurchaseController(
            IPurchasedCoursesService purchasedCoursesService,
            ICourseService courseService)
        {
            _purchasedCoursesService = purchasedCoursesService;
            _courseService = courseService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PurchaseCourseResponseDTO>> PurchaseCourse([FromBody] PurchaseCourseDTO purchaseDto)
        {
            if (purchaseDto == null)
                return BadRequest("Purchase data is required");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var course = await _courseService.GetCourseByIdAsync(purchaseDto.CourseId);
            if (course == null)
                return NotFound("Course not found");

            var hasPurchased = await _purchasedCoursesService.HasUserPurchasedCourseAsync(
                Guid.Parse(userId), 
                purchaseDto.CourseId);

            if (hasPurchased)
                return BadRequest("You have already purchased this course");

            var purchase = PurchasedCourses.FromDTO(purchaseDto, Guid.Parse(userId));

            var createdPurchase = await _purchasedCoursesService.AddPurchasedCourseAsync(purchase);
            if (createdPurchase == null)
                return BadRequest("Failed to create purchase record");

            return CreatedAtAction(
                nameof(GetPurchaseById),
                new { id = createdPurchase.Id },
                PurchaseCourseResponseDTO.FromPurchasedCourse(createdPurchase));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PurchaseCourseResponseDTO>> GetPurchaseById(Guid id)
        {
            var purchase = await _purchasedCoursesService.GetPurchasedCourseByIdAsync(id);
            if (purchase == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (purchase.UserId != Guid.Parse(userId) && !User.IsInRole(IdentityRoleConstants.Admin))
                return Forbid();

            return Ok(PurchaseCourseResponseDTO.FromPurchasedCourse(purchase));
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PurchaseCourseResponseDTO>>> GetUserPurchases()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var purchases = await _purchasedCoursesService.GetPurchasedCoursesByUserIdAsync(Guid.Parse(userId));

            var courseIds = purchases.Select(p => p.CourseId).ToList();

            var courses = new List<Course>();
            foreach (var courseId in courseIds)
            {
                var course = await _courseService.GetCourseByIdAsync(courseId);
                if (course != null)
                {
                    courses.Add(course);
                }
            }

            return Ok(courses);
        }

        [Authorize]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<PurchaseCourseResponseDTO>>> GetCoursePurchases(Guid courseId)
        {
            var purchases = await _purchasedCoursesService.GetPurchasedCoursesByCourseIdAsync(courseId);
            return Ok(purchases.Select(PurchaseCourseResponseDTO.FromPurchasedCourse));
        }

        [Authorize(Roles = IdentityRoleConstants.Admin)]
        [HttpPut("{id}/deactivate")]
        public async Task<ActionResult<PurchaseCourseResponseDTO>> DeactivatePurchase(Guid id)
        {
            var purchase = await _purchasedCoursesService.GetPurchasedCourseByIdAsync(id);
            if (purchase == null)
                return NotFound();

            purchase.IsActive = false;
            var updatedPurchase = await _purchasedCoursesService.UpdatePurchasedCourseAsync(purchase);
            if (updatedPurchase == null)
                return BadRequest("Failed to deactivate purchase");

            return Ok(PurchaseCourseResponseDTO.FromPurchasedCourse(updatedPurchase));
        }

        [Authorize(Roles = IdentityRoleConstants.Admin)]
        [HttpPut("{id}/activate")]
        public async Task<ActionResult<PurchaseCourseResponseDTO>> ActivatePurchase(Guid id)
        {
            var purchase = await _purchasedCoursesService.GetPurchasedCourseByIdAsync(id);
            if (purchase == null)
                return NotFound();

            purchase.IsActive = true;
            var updatedPurchase = await _purchasedCoursesService.UpdatePurchasedCourseAsync(purchase);
            if (updatedPurchase == null)
                return BadRequest("Failed to activate purchase");

            return Ok(PurchaseCourseResponseDTO.FromPurchasedCourse(updatedPurchase));
        }
    }
} 