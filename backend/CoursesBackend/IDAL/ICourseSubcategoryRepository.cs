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
        CourseSubcategory? GetCourseSubcategoryByID(Guid courseSubcategoryId);
        CourseSubcategory AddCourseSubcategory(CourseSubcategory courseSubcategory);
        CourseSubcategory? UpdateCourseSubcategory(CourseSubcategory courseSubcategory);
        CourseSubcategory? DeleteCourseSubcategory(Guid courseSubcategoryId);
    }
}
