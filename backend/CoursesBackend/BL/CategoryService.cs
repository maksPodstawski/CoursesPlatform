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

        public Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return _categoryRepository.GetCategoriesAsync();
        }
        public Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return _categoryRepository.GetCategoryByIDAsync(id);
        }
        public Task<Category?> GetCategoryByNameAsync(string name)
        {
            return _categoryRepository.GetCategoryByNameAsync(name);
        }
        public Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            return _categoryRepository.GetSubcategoriesByCategoryIdAsync(categoryId);
        }




        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddCategoryAsync(category);
            return category;
        }
        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            var existing = await _categoryRepository.GetCategoryByIDAsync(category.Id);
            if (existing == null)
                return null;

            await _categoryRepository.UpdateCategoryAsync(category);
            return category;
        }
        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            var category = await _categoryRepository.GetCategoryByIDAsync(id);
            if (category == null)
                return null;

            await _categoryRepository.DeleteCategoryAsync(id);
            return category;
        }
    }
}
