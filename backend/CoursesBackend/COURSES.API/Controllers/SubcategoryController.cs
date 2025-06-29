using Microsoft.AspNetCore.Mvc;
using IBL;
using Model.DTO;
using Model;

namespace COURSES.API.Controllers
{
    [ApiController]
    [Route("api/categories/{categoryId}/subcategories")]
    public class SubcategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public SubcategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubcategoryDTO>>> GetSubcategoriesByCategoryId(Guid categoryId)
        {
            var subcategories = await _categoryService.GetSubcategoriesByCategoryIdAsync(categoryId) ?? new List<Subcategory>(); ;
            var result = subcategories.Select(s => new SubcategoryDTO
            {
                Id = s.Id,
                Name = s.Name,
                CategoryId = s.CategoryId
            }).ToList();

            return Ok(result);
        }
    }
}