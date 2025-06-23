using Model.DTO;

namespace IBL
{
    public interface IAnalyticsService
    {
        Task<CreatorAnalyticsDTO> GetCreatorAnalyticsAsync(Guid creatorId, int year);
        Task<CreatorAnalyticsDTO> GetMyAnalyticsAsync(Guid userId);
        Task<CreatorAnalyticsDTO> GetMyAnalyticsAsync(Guid userId, int year);
        Task<List<CourseAnalyticsDTO>> GetCourseAnalyticsAsync(Guid creatorId);
        Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync(Guid creatorId, int year);
        Task<List<CoursePerformanceDTO>> GetTopPerformingCoursesAsync(Guid creatorId, int limit = 5);
    }
} 