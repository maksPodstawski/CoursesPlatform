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

        public Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return _courseRepository.GetCoursesAsync();
        }
        public Task<Course?> GetCourseByIdAsync(Guid id)
        {
            return _courseRepository.GetCourseByIDAsync(id);
        }
        public Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title)
        {
            return _courseRepository.GetCoursesByTitleAsync(title);
        }
        public async Task<IEnumerable<Course>> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _courseRepository.GetCoursesByPriceRangeAsync(minPrice, maxPrice);
        }
        public Task<IEnumerable<Course>> GetCoursesByAverageRatingAsync(double rating)
        {
            return _courseRepository.GetCoursesByAverageRatingAsync(rating);
        }
        public Task<IEnumerable<Course>> GetCoursesByCreatorAsync(Guid creatorId)
        {
            return _courseRepository.GetCoursesByCreatorAsync(creatorId);
        }




        public async Task<Course> AddCourseAsync(Course course)
        {
            await _courseRepository.AddCourseAsync(course);
            return course;
        }
        public async Task<Course?> UpdateCourseAsync(Course course)
        {
            var existing = await _courseRepository.GetCourseByIDAsync(course.Id);
            if (existing == null)
                return null;

            await _courseRepository.UpdateCourseAsync(course);
            return course;
        }
        public async Task<Course?> DeleteCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseByIDAsync(id);
            if (course == null)
                return null;

            await _courseRepository.DeleteCourseAsync(id);
            return course;
        }
    }
}
