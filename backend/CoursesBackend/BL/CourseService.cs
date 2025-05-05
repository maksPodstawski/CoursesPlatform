using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }


        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetCourses().ToListAsync();
        }
        public async Task<Course?> GetCourseByIdAsync(Guid id)
        {
            return await Task.FromResult(_courseRepository.GetCourseById(id));
        }
        public async Task<List<Course>> GetCoursesByTitleAsync(string title)
        {
            return await _courseRepository.GetCourses()
                .Where(c => c.Name.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        public async Task<List<Course>> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _courseRepository.GetCourses()
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .ToListAsync();
        }
        public async Task<List<Course>> GetCoursesByAverageRatingAsync(double rating)
        {
            return await _courseRepository.GetCourses()
                .Where(c => c.Reviews != null && c.Reviews.Any() && c.Reviews.Average(r => r.Rating) >= rating)
                .ToListAsync();
        }
        public async Task<List<Course>> GetCoursesByCreatorAsync(Guid creatorId)
        {
            return await _courseRepository.GetCourses()
                .Where(c => c.Stages != null && c.Stages.Any(s => s.CourseId == creatorId))
                .ToListAsync();
        }



        public async Task<Course> AddCourseAsync(Course course)
        {
            return await Task.FromResult(_courseRepository.AddCourse(course));
        }
        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            return await Task.FromResult(_courseRepository.UpdateCourse(course));
        }
        public async Task<Course?> DeleteCourseAsync(Guid id)
        {
            return await Task.FromResult(_courseRepository.DeleteCourse(id));
        }
    }
}
