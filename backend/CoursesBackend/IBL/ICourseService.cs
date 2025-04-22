using Model;

namespace IBL
{
    public interface ICourseService
    {
        IQueryable<Course> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid id);
        IQueryable<Course> GetCoursesByTitleAsync(string title);
        IQueryable<Course> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        IQueryable<Course> GetCoursesByAverageRatingAsync(double rating);
        IQueryable<Course> GetCoursesByCreatorAsync(Guid creatorId);
        Task<Course> AddCourseAsync(Course course);
        Task<Course?> UpdateCourseAsync(Course course);
        Task<Course?> DeleteCourseAsync(Guid id);
    }
}
