using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface ICourseSubcategoryRepository
    {
        Task<IEnumerable<CourseSubcategory>> GetCourseSubcategoriesAsync();
        Task<CourseSubcategory?> GetCourseSubcategoryByIDAsync(Guid courseSubcategoryID);
        Task AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task DeleteCourseSubcategoryAsync(Guid courseSubcategoryID);
    }
}
