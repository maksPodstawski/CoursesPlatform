using Model;

namespace IBL
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(Guid id);
        Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title);
        Task<IEnumerable<Course>> GetCoursesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Course>> GetCoursesByAverageRatingAsync(double rating);
        Task<IEnumerable<Course>> GetCoursesByCreatorAsync(Guid creatorId);



        Task<Course> AddCourseAsync(Course course);
        Task<Course?> UpdateCourseAsync(Course course);
        Task<Course?> DeleteCourseAsync(Guid id);
    }
}
