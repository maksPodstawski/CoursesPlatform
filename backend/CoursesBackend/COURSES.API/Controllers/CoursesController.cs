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
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICreatorService _creatorService;

        public CoursesController(ICourseService courseService, ICreatorService creatorService)
        {
            _courseService = courseService;
            _creatorService = creatorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDTO>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses.Select(CourseResponseDTO.FromCourse));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDTO>> GetCourseById(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(CourseResponseDTO.FromCourse(course));
        }

        [HttpGet("price-range")]
        public async Task<ActionResult<IEnumerable<CourseResponseDTO>>> GetCoursesByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var courses = await _courseService.GetCoursesByPriceRangeAsync(minPrice, maxPrice);
            return Ok(courses.Select(CourseResponseDTO.FromCourse));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CourseResponseDTO>> CreateCourse([FromBody] CreateCourseDTO createCourseDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                // Create the course
                var course = new Course
                {
                    Name = createCourseDto.Name,
                    Description = createCourseDto.Description,
                    ImageUrl = createCourseDto.ImageUrl,
                    Duration = createCourseDto.Duration,
                    Price = createCourseDto.Price,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCourse = await _courseService.AddCourseAsync(course);

                // Make the user a creator for this course
                var creator = await _creatorService.AddCreatorFromUserAsync(Guid.Parse(userId), createdCourse.Id);

                return CreatedAtAction(
                    nameof(GetCourseById), 
                    new { id = createdCourse.Id }, 
                    CourseResponseDTO.FromCourse(createdCourse)
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseResponseDTO>> UpdateCourse(Guid id, [FromBody] UpdateCourseDTO updateCourseDto)
        {
            var existingCourse = await _courseService.GetCourseByIdAsync(id);
            if (existingCourse == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Check if user is a creator of this course
            var isCreator = await _creatorService.IsUserCreatorOfCourseAsync(Guid.Parse(userId), id);
            if (!isCreator)
            {
                return Forbid();
            }

            existingCourse.Name = updateCourseDto.Name;
            existingCourse.Description = updateCourseDto.Description;
            existingCourse.ImageUrl = updateCourseDto.ImageUrl;
            existingCourse.Duration = updateCourseDto.Duration;
            existingCourse.Price = updateCourseDto.Price;
            existingCourse.UpdatedAt = DateTime.UtcNow;

            var updatedCourse = await _courseService.UpdateCourseAsync(existingCourse);
            if (updatedCourse == null)
            {
                return BadRequest("Failed to update course");
            }

            return Ok(CourseResponseDTO.FromCourse(updatedCourse));
        }
    }
} 