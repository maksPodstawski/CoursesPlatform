using BL.Exceptions;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DTO;

namespace BL.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICourseSubcategoryRepository _courseSubcategoryRepository;
        private readonly IProgressRepository _progressRepository;
        public CourseService(ICourseRepository courseRepository, ISubcategoryRepository subcategoryRepository, ICourseSubcategoryRepository courseSubcategoryRepository, IProgressRepository progressRepository)
        {
            _courseRepository = courseRepository;
            _subcategoryRepository = subcategoryRepository;
            _courseSubcategoryRepository = courseSubcategoryRepository;
            _progressRepository = progressRepository;
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

            if (await CourseTitleExistsAsync(course.Name))
                throw new CourseAlreadyExistsException(title: course.Name);

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

            var exists = await _courseRepository.GetCourses()
                .AnyAsync(c => c.Name.ToLower() == course.Name.ToLower() && c.Id != course.Id);

            if (exists)
                throw new CourseAlreadyExistsException(course.Name);

            return await Task.FromResult(_courseRepository.UpdateCourse(course));
        }

        public async Task<Course?> DeleteCourseAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(id));

            return await Task.FromResult(_courseRepository.DeleteCourse(id));
        }

        public async Task<List<Course>> GetVisibleCoursesAsync()
        {
            return await _courseRepository.GetCourses()
                .Where(c => !c.IsHidden)
                .ToListAsync();
        }

        public async Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId)
        {
            return _subcategoryRepository.GetSubcategoryByID(subcategoryId);
        }

        public async Task AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            _courseSubcategoryRepository.AddCourseSubcategory(courseSubcategory);
        }
        public async Task<bool> IsCourseCompletedAsync(Guid courseId, Guid userId)
        {
            var progresses = await _progressRepository.GetProgresses()
                .Where(p => p.UserId == userId && p.Stage.CourseId == courseId)
                .ToListAsync();

            var totalStages = progresses.Select(p => p.StageId).Distinct().Count();
            var completedStages = progresses.Count(p => p.IsCompleted);

            return totalStages > 0 && totalStages == completedStages;
        }
        public async Task RemoveCourseSubcategoryAsync(Guid courseSubcategoryId)
        {
            _courseSubcategoryRepository.DeleteCourseSubcategory(courseSubcategoryId);
            await Task.CompletedTask;
        }

        public async Task<bool> CourseTitleExistsAsync(string title)
        {
            return await _courseRepository.GetCourses()
                .AnyAsync(c => c.Name.ToLower() == title.ToLower());
        }
    }
}
