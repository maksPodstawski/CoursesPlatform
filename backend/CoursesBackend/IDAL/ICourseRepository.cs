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
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course?> GetCourseByIDAsync(Guid courseID);
        Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title);  
        Task<IEnumerable<Course>> GetCoursesByPriceAsync(decimal price);  
        Task<IEnumerable<Course>> GetCoursesByAverageRatingAsync(double rating);  
        Task<IEnumerable<Course>> GetCoursesByCreatorAsync(Guid creatorId);  


        Task AddCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(Guid courseID);
    }
}
