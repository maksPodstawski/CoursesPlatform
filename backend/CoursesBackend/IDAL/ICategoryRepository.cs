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
        Category? GetCategoryById(Guid categoryId);
        Category AddCategory(Category category);
        Category? UpdateCategory(Category category);
        Category? DeleteCategory(Guid categoryId);
    }
}
