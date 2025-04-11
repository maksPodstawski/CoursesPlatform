using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIDAsync(Guid categoryID);
        Task<Category?> GetCategoryByNameAsync(string name);  
        Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId);


        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid categoryID);
    }
}
