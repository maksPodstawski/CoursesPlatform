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
        IQueryable<Category> GetCategories();
        Task<Category?> GetCategoryByIdAsync(Guid categoryID);
        Task<Category?> GetCategoryByNameAsync(string name);  
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Guid categoryId);
    }
}
