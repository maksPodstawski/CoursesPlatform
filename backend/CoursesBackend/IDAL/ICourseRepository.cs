using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface ICourseRepository
    {
        IQueryable<Course> GetCourses();
        Course? GetCourseById(Guid courseId);
        Course AddCourse(Course course);
        Course? UpdateCourse(Course course);
        Course? DeleteCourse(Guid courseId);
    }
}
