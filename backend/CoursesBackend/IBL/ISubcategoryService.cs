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
        Task<List<Subcategory>> GetAllSubcategoriesAsync();
        Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId);
        Task<List<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId);
        Task<Subcategory> AddSubcategoryAsync(Subcategory subcategory);
        Task<Subcategory?> UpdateSubcategoryAsync(Subcategory subcategory);
        Task<Subcategory?> DeleteSubcategoryAsync(Guid subcategoryId);
    }
}
