using IBL;
using IDAL;
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

        public IQueryable<Course> GetAllCoursesAsync()
        {
            return _courseRepository.GetCourses();
        }
        public Task<Course?> GetCourseByIdAsync(Guid id)
        {
            return _courseRepository.GetCourseByIdAsync(id);
        }
        public IQueryable<Course> GetCoursesByTitleAsync(string title)
        {
            var courses =  _courseRepository.GetCourses();
            return courses.Where(c => c.Name.Contains(title, StringComparison.OrdinalIgnoreCase));
        }
        public IQueryable<Course> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var courses =  _courseRepository.GetCourses();
            return courses.Where(c => c.Price >= minPrice && c.Price <= maxPrice);
        }
        public IQueryable<Course> GetCoursesByAverageRatingAsync(double rating)
        {
            var courses =  _courseRepository.GetCourses();
            return courses.Where(c => c.Reviews != null && c.Reviews.Any() && c.Reviews.Average(r => r.Rating) >= rating);
        }
        public IQueryable<Course> GetCoursesByCreatorAsync(Guid creatorId)
        {
            var courses =  _courseRepository.GetCourses();
            return courses.Where(c => c.Stages != null && c.Stages.Any(s => s.CourseId == creatorId));
        }

        public async Task<Course> AddCourseAsync(Course course)
        {
            await _courseRepository.AddCourseAsync(course);
            return course;
        }
        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            var existing = await _courseRepository.GetCourseByIdAsync(course.Id);
            if (existing == null)
                return null;

            await _courseRepository.UpdateCourseAsync(course);
            return course;
        }
        public async Task<Course?> DeleteCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseByIdAsync(id);
            if (course == null)
                return null;

            await _courseRepository.DeleteCourseAsync(id);
            return course;
        }
    }
}
