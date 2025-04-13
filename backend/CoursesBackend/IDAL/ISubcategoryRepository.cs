using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface ISubcategoryRepository
    {
        Task<IEnumerable<Subcategory>> GetSubcategoriesAsync();
        Task<Subcategory> GetSubcategoryByIDAsync(Guid subcategoryID);
        Task InsertSubcategoryAsync(Subcategory subcategory);
        Task DeleteSubcategoryAsync(Guid subcategoryID);
        Task UpdateSubcategoryAsync(Subcategory subcategory);
        Task AddSubcategoryAsync(Subcategory subcategory);
    }
}
