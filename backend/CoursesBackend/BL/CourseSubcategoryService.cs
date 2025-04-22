using IBL;
using IDAL;
using Model;

namespace BL
{
    public class CourseSubcategoryService : ICourseSubcategoryService
    {
        private readonly ICourseSubcategoryRepository _courseSubcategoryRepository;
        public CourseSubcategoryService(ICourseSubcategoryRepository courseSubcategoryRepository)
        {
            _courseSubcategoryRepository = courseSubcategoryRepository;
        }

        public IQueryable<CourseSubcategory> GetAllCourseSubcategoriesAsync()
        {
            return _courseSubcategoryRepository.GetCourseSubcategories();
        }
        public Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid id)
        {
            return _courseSubcategoryRepository.GetCourseSubcategoryByIdAsync(id);
        }
        public async Task<CourseSubcategory> AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            await _courseSubcategoryRepository.AddCourseSubcategoryAsync(courseSubcategory);
            return courseSubcategory;
        }
        public async Task<CourseSubcategory?> UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            var existing = await _courseSubcategoryRepository.GetCourseSubcategoryByIdAsync(courseSubcategory.Id);
            if (existing == null)
                return null;

            await _courseSubcategoryRepository.UpdateCourseSubcategoryAsync(courseSubcategory);
            return courseSubcategory;
        }
        public async Task<CourseSubcategory?> DeleteCourseSubcategoryAsync(Guid id)
        {
            var existing = await _courseSubcategoryRepository.GetCourseSubcategoryByIdAsync(id);
            if (existing == null)
                return null;

            await _courseSubcategoryRepository.DeleteCourseSubcategoryAsync(id);
            return existing;
        }
    }
}
