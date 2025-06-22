using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Constans;
using IBL;
using Model;
using Model.DTO;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Authorize(Roles = IdentityRoleConstants.Admin)]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ISubcategoryService _subcategoryService;
        private readonly ICourseService _courseService;
        private readonly IReviewService _reviewService;

        public AdminController(
            ICategoryService categoryService,
            ISubcategoryService subcategoryService,
            ICourseService courseService,
            IReviewService reviewService)
        {
            _categoryService = categoryService;
            _subcategoryService = subcategoryService;
            _courseService = courseService;
            _reviewService = reviewService;
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok();
        }

        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest("Nazwa kategorii jest wymagana.");

            var category = new Category { Name = dto.Name };
            var created = await _categoryService.AddCategoryAsync(category);

            var resultDto = new CategoryDTO
            {
                Id = created.Id,
                Name = created.Name
            };

            return CreatedAtAction(nameof(AddCategory), new { id = resultDto.Id }, resultDto);
        }

        [HttpPost("subcategories")]
        public async Task<IActionResult> AddSubcategory([FromBody] SubcategoryNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest("Error! Subcategory name is required!");
            if (dto.CategoryId == Guid.Empty)
                return BadRequest("Error! Category ID is required!");

            var subcategory = new Subcategory
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId
            };
            var created = await _subcategoryService.AddSubcategoryAsync(subcategory);

            var resultDto = new SubcategoryDTO
            {
                Id = created.Id,
                Name = created.Name,
                CategoryId = created.CategoryId
            };

            return CreatedAtAction(nameof(AddSubcategory), new { id = resultDto.Id }, resultDto);
        }

        [HttpDelete("course/{courseId}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            var deleted = await _courseService.DeleteCourseAsync(courseId);
            if (deleted == null)
                return NotFound("Course not found");

            return Ok(new { message = "Course deleted", name = deleted.Name });
        }

        [HttpDelete("category/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(categoryId);
            if (deleted == null)
                return NotFound("Category not found");

            return Ok(new { message = "Category deleted", name = deleted.Name });
        }

        [HttpDelete("subcategory/{subcategoryId}")]
        public async Task<IActionResult> DeleteSubcategory(Guid subcategoryId)
        {
            var deleted = await _subcategoryService.DeleteSubcategoryAsync(subcategoryId);
            if (deleted == null)
                return NotFound("Subcategory not found");

            return Ok(new { message = "Subcategory deleted", name = deleted.Name });
        }

    }
}

