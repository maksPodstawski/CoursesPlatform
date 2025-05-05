using Model;

namespace IBL
{
    public interface ICourseSubcategoryService
    {
        Task<List<CourseSubcategory>> GetAllCourseSubcategoriesAsync();
        Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid id);
        Task<List<CourseSubcategory>> GetByCourseIdAsync(Guid courseId);
        Task<List<CourseSubcategory>> GetBySubcategoryIdAsync(Guid subcategoryId);
        Task<CourseSubcategory> AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task<CourseSubcategory?> UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task<CourseSubcategory?> DeleteCourseSubcategoryAsync(Guid id);
    }
}
