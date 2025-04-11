using Model;

namespace IBL
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(Guid id);
        Task<Category?> GetCategoryByNameAsync(string name); 
        Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId);


        Task<Category> AddCategoryAsync(Category category);
        Task<Category?> UpdateCategoryAsync(Category category);
        Task<Category?> DeleteCategoryAsync(Guid id);
    }
}
