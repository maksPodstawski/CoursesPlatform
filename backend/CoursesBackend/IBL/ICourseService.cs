using Model;

namespace IBL
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid id);
        Task<List<Course>> GetCoursesByTitleAsync(string title);
        Task<List<Course>> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<List<Course>> GetCoursesByAverageRatingAsync(double rating);
        Task<List<Course>> GetCoursesByCreatorAsync(Guid creatorId);
        Task<Course> AddCourseAsync(Course course);
        Task<Course?> UpdateCourseAsync(Course course);
        Task<Course?> DeleteCourseAsync(Guid id);
        Task<List<Course>> GetVisibleCoursesAsync();
        Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId);
        Task AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory);
    }
}
