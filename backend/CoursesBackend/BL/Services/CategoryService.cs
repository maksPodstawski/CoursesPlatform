using BL.Exceptions;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        public CategoryService(ICategoryRepository categoryRepository, ISubcategoryRepository subcategoryRepository)
        {
            _categoryRepository = categoryRepository;
            _subcategoryRepository = subcategoryRepository;
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
                .FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
        }
        public async Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            return await Task.FromResult(_subcategoryRepository.GetSubcategories().Where(s => s.CategoryId == categoryId).ToList());
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if(string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be null or empty.", nameof(category.Name));

            var exists = await _categoryRepository.GetCategories()
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());

            if (exists)
                throw new CategoryAlreadyExistsException(category.Name);

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

            var exists = await _categoryRepository.GetCategories()
                .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id);

            if (exists)
                throw new CategoryAlreadyExistsException(category.Name);

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
