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
        Task<Course?> GetCourseByIdAsync(Guid courseId);
        Task AddCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(Guid courseId);
    }
}
