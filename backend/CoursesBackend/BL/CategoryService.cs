using IBL;
using IDAL;
using Model;

namespace BL
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IQueryable<Category> GetAllCategoriesAsync()
        {
            return _categoryRepository.GetCategories();
        }
        public Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return _categoryRepository.GetCategoryByIdAsync(id);
        }
        public Task<Category?> GetCategoryByNameAsync(string name)
        {
            return _categoryRepository.GetCategoryByNameAsync(name);
        }
        public async Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {

            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (category == null || category.Subcategories == null)
            {
                return Enumerable.Empty<Subcategory>();
            }

            return category.Subcategories;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddCategoryAsync(category);
            return category;
        }
        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            var existing = await _categoryRepository.GetCategoryByIdAsync(category.Id);
            if (existing == null)
                return null;

            await _categoryRepository.UpdateCategoryAsync(category);
            return category;
        }
        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                return null;

            await _categoryRepository.DeleteCategoryAsync(id);
            return category;
        }
    }
}
