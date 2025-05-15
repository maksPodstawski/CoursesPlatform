using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetCategories().ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await Task.FromResult(_categoryRepository.GetCategoryById(id));
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _categoryRepository.GetCategories()
                .FirstOrDefaultAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            var category = _categoryRepository.GetCategoryById(categoryId);
            if (category == null || category.Subcategories == null)
                return Enumerable.Empty<Subcategory>();

            return await Task.FromResult(category.Subcategories);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if(string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(category.Name));

            var result = _categoryRepository.AddCategory(category);
            if (result == null)
                throw new InvalidOperationException("Failed to add category. Repository returned null.");

            return await Task.FromResult(result);
        }

        public async Task<Category?> UpdateCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(category.Name));

            return await Task.FromResult(_categoryRepository.UpdateCategory(category));
        }

        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Category ID cannot be empty.", nameof(id));

            return await Task.FromResult(_categoryRepository.DeleteCategory(id));
        }
    }
}
