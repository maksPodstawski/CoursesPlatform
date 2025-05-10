using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
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
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            return await _courseRepository.GetCourses()
                .Where(c => c.Name.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }

        public async Task<List<Course>> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot be greater than maximum price.");

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
            if (creatorId == Guid.Empty)
                throw new ArgumentException("Creator ID cannot be empty.", nameof(creatorId));

            return await _courseRepository.GetCourses()
                .Where(c => c.Stages != null && c.Stages.Any(s => s.CourseId == creatorId))
                .ToListAsync();
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            var result = _courseRepository.AddCourse(course);
            if (result == null)
                throw new InvalidOperationException("Failed to add course. Repository returned null.");

            return await Task.FromResult(result);
        }

        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            if(course.Id == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(course.Id));

            return await Task.FromResult(_courseRepository.UpdateCourse(course));
        }

        public async Task<Course?> DeleteCourseAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(id));

            return await Task.FromResult(_courseRepository.DeleteCourse(id));
        }
    }
}
