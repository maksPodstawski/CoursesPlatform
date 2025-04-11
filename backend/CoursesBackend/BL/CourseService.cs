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
