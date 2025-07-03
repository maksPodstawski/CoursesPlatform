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
        private readonly IPurchasedCoursesService _purchasedCoursesService;

        public CoursesController(ICourseService courseService, ICreatorService creatorService, IPurchasedCoursesService purchasedCoursesService)
        {
            _courseService = courseService;
            _creatorService = creatorService;
            _purchasedCoursesService = purchasedCoursesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDTO>>> GetAllVisibleCourses()
        {
            var courses = await _courseService.GetVisibleCoursesAsync();
            return Ok(courses.Select(CourseResponseDTO.FromCourse));
        }


        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("{courseId}/instructor")]
        public async Task<ActionResult<InstructorResponseDTO>> GetCourseInstructor(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found");
            }

            var creators = await _creatorService.GetCreatorsByCourseAsync(courseId);
            var creator = creators.FirstOrDefault();
            if (creator == null)
            {
                return NotFound("Course instructor not found");
            }

            return Ok(new InstructorResponseDTO
            {
                Name = creator.User.ToString(),
                Title = creator.Title
            });
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
                var course = Course.FromCreateDTO(createCourseDto);

                var createdCourse = await _courseService.AddCourseAsync(course);

                if (createCourseDto.SubcategoryIds != null && createCourseDto.SubcategoryIds.Any())
                {
                    var validSubcategories = new List<Subcategory>();
                    foreach (var subId in createCourseDto.SubcategoryIds)
                    {
                        var subcategory = await _courseService.GetSubcategoryByIdAsync(subId);
                        if (subcategory != null)
                        {
                            validSubcategories.Add(subcategory);
                        }
                    }
                    if (validSubcategories.Count != createCourseDto.SubcategoryIds.Count)
                    {
                        return BadRequest("One or more subcategories do not exist.");
                    }
                    foreach (var subcategory in validSubcategories)
                    {
                        var courseSubcategory = new CourseSubcategory
                        {
                            Id = Guid.NewGuid(),
                            CourseId = createdCourse.Id,
                            SubcategoryId = subcategory.Id
                        };
                        await _courseService.AddCourseSubcategoryAsync(courseSubcategory);
                    }
                }

                var creator = await _creatorService.AddCreatorFromUserAsync(Guid.Parse(userId), createdCourse.Id);

                var purchaseDto = new PurchaseCourseDTO
                {
                    CourseId = createdCourse.Id,
                    Price = createdCourse.Price,
                    ExpirationDate = null  
                };

                var purchase = PurchasedCourses.FromDTO(purchaseDto, Guid.Parse(userId));
                await _purchasedCoursesService.AddPurchasedCourseAsync(purchase);

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
            existingCourse.IsHidden = updateCourseDto.IsHidden;
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