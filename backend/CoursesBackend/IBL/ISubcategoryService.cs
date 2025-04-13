using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface ISubcategoryService
    {
        Task<IEnumerable<Subcategory>> GetAllSubcategoriesAsync();
        Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId);
        Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId);
        Task AddSubcategoryAsync(Subcategory subcategory);
        Task UpdateSubcategoryAsync(Subcategory subcategory);
        Task DeleteSubcategoryAsync(Guid subcategoryId);
    }
}
