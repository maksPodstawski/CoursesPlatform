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
        IQueryable<CourseSubcategory> GetCourseSubcategories();
        Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid courseSubcategoryId);
        Task AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task DeleteCourseSubcategoryAsync(Guid courseSubcategoryId);
    }
}
