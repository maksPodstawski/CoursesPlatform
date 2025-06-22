using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DTO;

namespace BL.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ICreatorRepository _creatorRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IPurchasedCoursesRepository _purchasedCoursesRepository;
        private readonly IProgressRepository _progressRepository;
        private readonly IReviewRepository _reviewRepository;

        public AnalyticsService(
            ICreatorRepository creatorRepository,
            ICourseRepository courseRepository,
            IPurchasedCoursesRepository purchasedCoursesRepository,
            IProgressRepository progressRepository,
            IReviewRepository reviewRepository)
        {
            _creatorRepository = creatorRepository;
            _courseRepository = courseRepository;
            _purchasedCoursesRepository = purchasedCoursesRepository;
            _progressRepository = progressRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<CreatorAnalyticsDTO> GetCreatorAnalyticsAsync(Guid creatorId, int year)
        {
            var creator = await _creatorRepository.GetCreators()
                .Include(c => c.User)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Stages)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Reviews)
                .FirstOrDefaultAsync(c => c.Id == creatorId);

            if (creator == null)
                throw new ArgumentException("Creator not found");

            var courses = creator.Courses.ToList();
            var courseIds = courses.Select(c => c.Id).ToList();

            var purchases = await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => courseIds.Contains(pc.CourseId) && pc.IsActive)
                .ToListAsync();

            var stageIds = courses.SelectMany(c => c.Stages).Select(s => s.Id).ToList();
            var progressData = await _progressRepository.GetProgresses()
                .Where(p => stageIds.Contains(p.StageId))
                .ToListAsync();

            var totalStudents = purchases.Select(p => p.UserId).Distinct().Count();
            var totalRevenue = purchases.Sum(p => p.PurchasedPrice);
            var totalReviews = courses.Sum(c => c.Reviews?.Count ?? 0);
            var averageRating = courses
                .Where(c => c.Reviews?.Any() == true)
                .SelectMany(c => c.Reviews)
                .Average(r => r.Rating);
            var totalStages = courses.Sum(c => c.Stages?.Count ?? 0);
            
            double overallCompletionRate = 0;
            if (totalStudents > 0)
            {
                var usersWhoCompletedAllStages = 0;
                
                foreach (var course in courses)
                {
                    var coursePurchases = purchases.Where(p => p.CourseId == course.Id).ToList();
                    var courseStageIds = course.Stages.Select(s => s.Id).ToList();
                    var courseProgress = progressData.Where(p => courseStageIds.Contains(p.StageId)).ToList();
                    
                    foreach (var purchase in coursePurchases)
                    {
                        var userProgress = courseProgress.Where(p => p.UserId == purchase.UserId).ToList();
                        var userCompletedStages = userProgress.Where(p => p.IsCompleted).Select(p => p.StageId).Distinct().Count();
                        
                        if (userCompletedStages == courseStageIds.Count)
                        {
                            usersWhoCompletedAllStages++;
                        }
                    }
                }
                
                overallCompletionRate = (double)usersWhoCompletedAllStages / totalStudents * 100;
            }

            var monthlyRevenue = await GetMonthlyRevenueAsync(creatorId, year);

            var topPerformingCourses = await GetTopPerformingCoursesAsync(creatorId);

            var courseAnalytics = await GetCourseAnalyticsAsync(creatorId);

            return new CreatorAnalyticsDTO
            {
                CreatorId = creator.Id,
                CreatorName = creator.User.ToString(),
                TotalCourses = courses.Count,
                TotalStudents = totalStudents,
                TotalRevenue = totalRevenue,
                AverageRating = averageRating,
                TotalReviews = totalReviews,
                TotalStages = totalStages,
                CompletedStages = (int)Math.Round(overallCompletionRate),
                Courses = courseAnalytics,
                MonthlyRevenue = monthlyRevenue,
                TopPerformingCourses = topPerformingCourses
            };
        }

        public async Task<CreatorAnalyticsDTO> GetMyAnalyticsAsync(Guid userId)
        {
            return await GetMyAnalyticsAsync(userId, DateTime.UtcNow.Year);
        }
        
        public async Task<CreatorAnalyticsDTO> GetMyAnalyticsAsync(Guid userId, int year)
        {
            var creator = await _creatorRepository.GetCreators()
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (creator == null)
            {
                return new CreatorAnalyticsDTO
                {
                    CreatorId = Guid.Empty,
                    CreatorName = "Not a creator yet",
                    TotalCourses = 0,
                    TotalStudents = 0,
                    TotalRevenue = 0,
                    AverageRating = 0,
                    TotalReviews = 0,
                    TotalStages = 0,
                    CompletedStages = 0,
                    Courses = new List<CourseAnalyticsDTO>(),
                    MonthlyRevenue = new List<MonthlyRevenueDTO>(),
                    TopPerformingCourses = new List<CoursePerformanceDTO>()
                };
            }

            return await GetCreatorAnalyticsAsync(creator.Id, year);
        }

        public async Task<List<CourseAnalyticsDTO>> GetCourseAnalyticsAsync(Guid creatorId)
        {
            var courses = await _creatorRepository.GetCreators()
                .Where(c => c.Id == creatorId)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Stages)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Reviews)
                .SelectMany(c => c.Courses)
                .ToListAsync();

            var courseIds = courses.Select(c => c.Id).ToList();
            var stageIds = courses.SelectMany(c => c.Stages).Select(s => s.Id).ToList();

            var purchases = await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => courseIds.Contains(pc.CourseId) && pc.IsActive)
                .ToListAsync();

            var progressData = await _progressRepository.GetProgresses()
                .Where(p => stageIds.Contains(p.StageId))
                .ToListAsync();

            var result = new List<CourseAnalyticsDTO>();

            foreach (var course in courses)
            {
                var coursePurchases = purchases.Where(p => p.CourseId == course.Id).ToList();
                var courseStageIds = course.Stages.Select(s => s.Id).ToList();
                var courseProgress = progressData.Where(p => courseStageIds.Contains(p.StageId)).ToList();

                var studentsCount = coursePurchases.Select(p => p.UserId).Distinct().Count();
                var revenue = coursePurchases.Sum(p => p.PurchasedPrice);
                var averageRating = course.Reviews?.Any() == true ? course.Reviews.Average(r => r.Rating) : 0;
                var reviewsCount = course.Reviews?.Count ?? 0;
                var stagesCount = course.Stages?.Count ?? 0;

                double courseCompletionRate = 0;
                if (studentsCount > 0 && stagesCount > 0)
                {
                    var usersWhoCompletedAllStages = 0;
                    
                    foreach (var purchase in coursePurchases)
                    {
                        var userProgress = courseProgress.Where(p => p.UserId == purchase.UserId).ToList();
                        var userCompletedStages = userProgress.Where(p => p.IsCompleted).Select(p => p.StageId).Distinct().Count();
                        
                        if (userCompletedStages == stagesCount)
                        {
                            usersWhoCompletedAllStages++;
                        }
                    }
                    
                    courseCompletionRate = (double)usersWhoCompletedAllStages / studentsCount * 100;
                }

                var stageAnalytics = course.Stages?.Select(stage =>
                {
                    var stageProgress = courseProgress.Where(p => p.StageId == stage.Id).ToList();
                    var studentsStarted = stageProgress.Count;
                    var studentsCompleted = stageProgress.Count(p => p.IsCompleted);
                    var completionRate = studentsStarted > 0 ? (double)studentsCompleted / studentsStarted * 100 : 0;
                    var averageTimeToComplete = stageProgress
                        .Where(p => p.IsCompleted && p.CompletedAt.HasValue)
                        .Select(p => (p.CompletedAt.Value - p.StartedAt).TotalMinutes)
                        .DefaultIfEmpty(0)
                        .Average();

                    return new StageAnalyticsDTO
                    {
                        StageId = stage.Id,
                        StageName = stage.Name,
                        StudentsStarted = studentsStarted,
                        StudentsCompleted = studentsCompleted,
                        CompletionRate = completionRate,
                        AverageTimeToComplete = averageTimeToComplete
                    };
                }).ToList() ?? new List<StageAnalyticsDTO>();

                result.Add(new CourseAnalyticsDTO
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    StudentsCount = studentsCount,
                    Revenue = revenue,
                    AverageRating = averageRating,
                    ReviewsCount = reviewsCount,
                    StagesCount = stagesCount,
                    CompletedStagesCount = (int)Math.Round(courseCompletionRate),
                    CreatedAt = course.CreatedAt,
                    Stages = stageAnalytics
                });
            }

            return result;
        }

        public async Task<List<MonthlyRevenueDTO>> GetMonthlyRevenueAsync(Guid creatorId, int year)
        {
            var courses = await _creatorRepository.GetCreators()
                .Where(c => c.Id == creatorId)
                .Include(c => c.Courses)
                .SelectMany(c => c.Courses)
                .ToListAsync();

            var courseIds = courses.Select(c => c.Id).ToList();

            var purchases = await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => courseIds.Contains(pc.CourseId) && 
                            pc.IsActive && 
                            pc.PurchasedAt.Year == year)
                .ToListAsync();

            var monthlyData = purchases
                .GroupBy(p => new { p.PurchasedAt.Year, p.PurchasedAt.Month })
                .Select(g => new MonthlyRevenueDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(p => p.PurchasedPrice),
                    SalesCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return monthlyData;
        }

        public async Task<List<CoursePerformanceDTO>> GetTopPerformingCoursesAsync(Guid creatorId, int limit = 5)
        {
            var courses = await _creatorRepository.GetCreators()
                .Where(c => c.Id == creatorId)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Stages)
                .Include(c => c.Courses)
                .ThenInclude(course => course.Reviews)
                .SelectMany(c => c.Courses)
                .ToListAsync();

            var courseIds = courses.Select(c => c.Id).ToList();
            var stageIds = courses.SelectMany(c => c.Stages).Select(s => s.Id).ToList();

            var purchases = await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => courseIds.Contains(pc.CourseId) && pc.IsActive)
                .ToListAsync();

            var progressData = await _progressRepository.GetProgresses()
                .Where(p => stageIds.Contains(p.StageId))
                .ToListAsync();

            var performanceData = new List<CoursePerformanceDTO>();

            foreach (var course in courses)
            {
                var coursePurchases = purchases.Where(p => p.CourseId == course.Id).ToList();
                var courseStageIds = course.Stages.Select(s => s.Id).ToList();
                var courseProgress = progressData.Where(p => courseStageIds.Contains(p.StageId)).ToList();

                var studentsCount = coursePurchases.Select(p => p.UserId).Distinct().Count();
                var revenue = coursePurchases.Sum(p => p.PurchasedPrice);
                var averageRating = course.Reviews?.Any() == true ? course.Reviews.Average(r => r.Rating) : 0;
                
                var totalStages = course.Stages?.Count ?? 0;
                
                double completionRate = 0;
                if (studentsCount > 0 && totalStages > 0)
                {
                    var usersWhoCompletedAllStages = 0;
                    
                    foreach (var purchase in coursePurchases)
                    {
                        var userProgress = courseProgress.Where(p => p.UserId == purchase.UserId).ToList();
                        var userCompletedStages = userProgress.Where(p => p.IsCompleted).Select(p => p.StageId).Distinct().Count();
                        
                        if (userCompletedStages == totalStages)
                        {
                            usersWhoCompletedAllStages++;
                        }
                    }
                    
                    completionRate = (double)usersWhoCompletedAllStages / studentsCount * 100;
                }

                performanceData.Add(new CoursePerformanceDTO
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    StudentsCount = studentsCount,
                    Revenue = revenue,
                    AverageRating = averageRating,
                    CompletionRate = completionRate
                });
            }

            return performanceData
                .OrderByDescending(p => p.Revenue)
                .ThenByDescending(p => p.StudentsCount)
                .Take(limit)
                .ToList();
        }
    }
} 