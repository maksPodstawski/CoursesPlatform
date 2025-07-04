using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IBL;
using Model;
using Model.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;

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
        public async Task<ActionResult<CourseResponseDTO>> CreateCourse([FromForm] CreateCourseWithImageDTO createCourseDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var course = new Course
                {
                    Name = createCourseDto.Name,
                    Description = createCourseDto.Description,
                    ImageUrl = "", 
                    Duration = createCourseDto.Duration,
                    Price = createCourseDto.Price,
                    IsHidden = createCourseDto.IsHidden,
                    CreatedAt = DateTime.UtcNow
                };

                var createdCourse = await _courseService.AddCourseAsync(course);

                if (createCourseDto.Image != null && createCourseDto.Image.Length > 0)
                {
                    var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", createdCourse.Id.ToString());
                    if (!Directory.Exists(uploadsRoot))
                        Directory.CreateDirectory(uploadsRoot);

                    var fileName = Path.GetFileName(createCourseDto.Image.FileName);
                    var filePath = Path.Combine(uploadsRoot, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await createCourseDto.Image.CopyToAsync(stream);
                    }
                    createdCourse.ImageUrl = $"/UploadedImages/{createdCourse.Id}/{fileName}";
                    await _courseService.UpdateCourseAsync(createdCourse);
                }

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

                var purchase = new PurchasedCourses
                {
                    UserId = Guid.Parse(userId),
                    CourseId = createdCourse.Id,
                    PurchasedPrice = createdCourse.Price,
                    PurchasedAt = DateTime.UtcNow,
                    IsActive = true
                };
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

        [HttpGet("{courseId}/image/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetCourseImage(Guid courseId, string fileName)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages", courseId.ToString(), fileName);
            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
            return PhysicalFile(imagePath, contentType);
        }
    }
}