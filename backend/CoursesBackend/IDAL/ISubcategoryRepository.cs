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
        Subcategory GetSubcategoryByID(Guid subcategoryID);
        Subcategory? DeleteSubcategory(Guid subcategoryID);
        Subcategory? UpdateSubcategory(Subcategory subcategory);
        Subcategory AddSubcategory(Subcategory subcategory);
    }
}
