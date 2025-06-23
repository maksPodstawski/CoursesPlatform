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
        private readonly ISubcategoryService _subcategoryService;

        public SubcategoryController(ISubcategoryService subcategoryService)    
        {
            _subcategoryService = subcategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubcategoryDTO>>> GetSubcategoriesByCategoryId(Guid categoryId)
        {
            Console.WriteLine($" Called with categoryId: {categoryId}");

            var subcategories = await _subcategoryService.GetSubcategoriesByCategoryIdAsync(categoryId);

            Console.WriteLine($" Subcategories found: {subcategories.Count}");

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