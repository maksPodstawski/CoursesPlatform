namespace Model.DTO
{
    public record CreatorAnalyticsDTO
    {
        public Guid CreatorId { get; init; }
        public string CreatorName { get; init; } = string.Empty;
        public int TotalCourses { get; init; }
        public int TotalStudents { get; init; }
        public decimal TotalRevenue { get; init; }
        public double AverageRating { get; init; }
        public int TotalReviews { get; init; }
        public int TotalStages { get; init; }
        public int CompletedStages { get; init; }
        public List<CourseAnalyticsDTO> Courses { get; init; } = new();
        public List<MonthlyRevenueDTO> MonthlyRevenue { get; init; } = new();
        public List<CoursePerformanceDTO> TopPerformingCourses { get; init; } = new();
    }

    public record CourseAnalyticsDTO
    {
        public Guid CourseId { get; init; }
        public string CourseName { get; init; } = string.Empty;
        public int StudentsCount { get; init; }
        public decimal Revenue { get; init; }
        public double AverageRating { get; init; }
        public int ReviewsCount { get; init; }
        public int StagesCount { get; init; }
        public int CompletedStagesCount { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<StageAnalyticsDTO> Stages { get; init; } = new();
    }

    public record StageAnalyticsDTO
    {
        public Guid StageId { get; init; }
        public string StageName { get; init; } = string.Empty;
        public int StudentsStarted { get; init; }
        public int StudentsCompleted { get; init; }
        public double CompletionRate { get; init; }
        public double AverageTimeToComplete { get; init; }
    }

    public record MonthlyRevenueDTO
    {
        public int Year { get; init; }
        public int Month { get; init; }
        public decimal Revenue { get; init; }
        public int SalesCount { get; init; }
    }

    public record CoursePerformanceDTO
    {
        public Guid CourseId { get; init; }
        public string CourseName { get; init; } = string.Empty;
        public int StudentsCount { get; init; }
        public decimal Revenue { get; init; }
        public double AverageRating { get; init; }
        public double CompletionRate { get; init; }
    }
} 