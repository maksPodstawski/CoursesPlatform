using IBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Constans;
using Model.DTO;
using System.Text.Json;

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
        private readonly string _perspectiveApiKey;

        public AdminController(
            ICategoryService categoryService,
            ISubcategoryService subcategoryService,
            ICourseService courseService,
            IReviewService reviewService,
            IConfiguration configuration)
        {
            _categoryService = categoryService;
            _subcategoryService = subcategoryService;
            _courseService = courseService;
            _reviewService = reviewService;
            _perspectiveApiKey = configuration["GooglePerspective:ApiKey"] ??
                                 throw new ArgumentNullException("GooglePerspective:ApiKey is not configured.");
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
                return BadRequest(new { message = "Category name is required." });

            var exists = await _categoryService.GetCategoryByNameAsync(dto.Name.Trim());
            if (exists != null)
                return BadRequest(new { message = "A category with this name already exists." });

            var category = new Category { Name = dto.Name.Trim() };
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
                return BadRequest(new { message = "Subcategory name is required." });
            if (dto.CategoryId == Guid.Empty)
                return BadRequest(new { message = "Category ID is required." });

            var exists = await _subcategoryService.GetSubcategoryByNameAsync(dto.Name.Trim(), dto.CategoryId);
            if (exists != null)
                return BadRequest(new { message = "A subcategory with this name already exists in this category." });

            var subcategory = new Subcategory
            {
                Name = dto.Name.Trim(),
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

        [HttpPut("category/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest(new { message = "Category name is required." });

            var exists = await _categoryService.GetCategoryByNameAsync(dto.Name.Trim());
            if (exists != null && exists.Id != id)
                return BadRequest(new { message = "A category with this name already exists." });

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            category.Name = dto.Name.Trim();
            var updated = await _categoryService.UpdateCategoryAsync(category);

            var resultDto = new CategoryDTO
            {
                Id = updated.Id,
                Name = updated.Name
            };

            return Ok(resultDto);
        }

        [HttpPut("subcategory/{id}")]
        public async Task<IActionResult> UpdateSubcategory(Guid id, [FromBody] SubcategoryNameDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
                return BadRequest(new { message = "Subcategory name is required." });

            var subcategory = await _subcategoryService.GetSubcategoryByIdAsync(id);
            if (subcategory == null)
                return NotFound(new { message = "Subcategory not found." });

            var exists = await _subcategoryService.GetSubcategoryByNameAsync(dto.Name.Trim(), subcategory.CategoryId);
            if (exists != null && exists.Id != id)
                return BadRequest(new { message = "A subcategory with this name already exists in this category." });

            subcategory.Name = dto.Name.Trim();
            var updated = await _subcategoryService.UpdateSubcategoryAsync(subcategory);

            var resultDto = new SubcategoryDTO
            {
                Id = updated.Id,
                Name = updated.Name,
                CategoryId = updated.CategoryId
            };

            return Ok(resultDto);
        }

        [HttpDelete("course/{courseId}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            var deleted = await _courseService.DeleteCourseAsync(courseId);
            if (deleted == null)
                return NotFound(new { message = "Course not found." });

            return Ok(new DeleteResponseDTO
            {
                message = "Course deleted",
                name = deleted.Name
            });
        }

        [HttpDelete("category/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(categoryId);
            if (deleted == null)
                return NotFound(new { message = "Category not found." });

            return Ok(new { message = "Category deleted", name = deleted.Name });
        }

        [HttpDelete("subcategory/{subcategoryId}")]
        public async Task<IActionResult> DeleteSubcategory(Guid subcategoryId)
        {
            var deleted = await _subcategoryService.DeleteSubcategoryAsync(subcategoryId);
            if (deleted == null)
                return NotFound(new { message = "Subcategory not found." });

            return Ok(new { message = "Subcategory deleted", name = deleted.Name });
        }

        [HttpPut("courses/{id}/toggle-visibility")]
        public async Task<IActionResult> ToggleCourseVisibility(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound(new { message = "Course not found." });

            course.IsHidden = !course.IsHidden;
            var updated = await _courseService.UpdateCourseAsync(course);
            return Ok(CourseResponseDTO.FromCourse(updated));
        }

        [HttpDelete("review/{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Review not found." });

            var deleted = await _reviewService.DeleteReviewAsync(id);
            if (deleted == null)
                return BadRequest(new { message = "Failed to delete review." });

            return NoContent();
        }

        [HttpGet("toxic-review-count")]
        public async Task<IActionResult> GetToxicReviewCount()
        {
            var allReviews = await _reviewService.GetAllReviewsAsync();
            int toxicCount = 0;

            foreach (var review in allReviews)
            {
                var score = await AnalyzeToxicityAsync(review.Comment);
                if (score > 0.2) toxicCount++;
            }

            return Ok(new { count = toxicCount });
        }

        private async Task<double> AnalyzeToxicityAsync(string comment)
        {
            using var httpClient = new HttpClient();

            var request = new Dictionary<string, object>
            {
                ["comment"] = new { text = comment },
                ["requestedAttributes"] = new Dictionary<string, object>
                {
                    ["TOXICITY"] = new { }
                }
            };

            var response = await httpClient.PostAsJsonAsync(
                $"https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key={_perspectiveApiKey}",
                request);

            var raw = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ToxicityResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result?.attributeScores?["TOXICITY"]?.summaryScore?.value ?? 0.0;
        }

        [HttpPost("analyze-toxicity")]
        public async Task<IActionResult> AnalyzeToxicity([FromBody] AnalyzeReviewDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Comment))
                return BadRequest(new { message = "The comment field is required." });

            var score = await AnalyzeToxicityAsync(dto.Comment);
            return Ok(new { score });
        }

        // Classes for deserialization of Perspective API response
        private class ToxicityResponse
        {
            public Dictionary<string, AttributeScore>? attributeScores { get; set; }
        }

        private class AttributeScore
        {
            public SummaryScore? summaryScore { get; set; }
        }

        private class SummaryScore
        {
            public double value { get; set; }
            public string type { get; set; } = string.Empty;
        }
    }
}
