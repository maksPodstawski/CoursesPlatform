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
        IQueryable<Subcategory> GetSubcategories();
        Task<Subcategory> GetSubcategoryByIDAsync(Guid subcategoryID);
        Task DeleteSubcategoryAsync(Guid subcategoryID);
        Task UpdateSubcategoryAsync(Subcategory subcategory);
        Task AddSubcategoryAsync(Subcategory subcategory);
    }
}
