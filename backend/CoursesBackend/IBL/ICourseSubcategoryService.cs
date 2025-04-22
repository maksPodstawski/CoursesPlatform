using Model;

namespace IBL
{
    public interface ICourseSubcategoryService
    {
        IQueryable<CourseSubcategory> GetAllCourseSubcategoriesAsync();
        Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid id);
        Task<CourseSubcategory> AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task<CourseSubcategory?> UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
        Task<CourseSubcategory?> DeleteCourseSubcategoryAsync(Guid id);
    }
}
