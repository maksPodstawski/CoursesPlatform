using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public record CreateCourseDTO
    {
        [Required]
        public required string Name { get; init; }
        public string? Description { get; init; }
        [Required]
        public required string ImageUrl { get; init; }
        [Required]
        public int Duration { get; init; }
        [Required]
        public decimal Price { get; init; }
    }

    public record UpdateCourseDTO
    {
        [Required]
        public required string Name { get; init; }
        public string? Description { get; init; }
        [Required]
        public required string ImageUrl { get; init; }
        [Required]
        public int Duration { get; init; }
        [Required]
        public decimal Price { get; init; }
    }

    public record CourseResponseDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public int Duration { get; init; }
        public decimal Price { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public double? AverageRating { get; init; }
        public int ReviewsCount { get; init; }
        public int StagesCount { get; init; }
        public List<string> Subcategories { get; init; } = new();
        public List<string> Creators { get; init; } = new();

        public static CourseResponseDTO FromCourse(Course course)
        {
            return new CourseResponseDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                ImageUrl = course.ImageUrl,
                Duration = course.Duration,
                Price = course.Price,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                AverageRating = course.Reviews?.Any() == true ? course.Reviews.Average(r => r.Rating) : null,
                ReviewsCount = course.Reviews?.Count ?? 0,
                StagesCount = course.Stages?.Count ?? 0,
                Subcategories = course.CourseSubcategories?.Select(cs => cs.Subcategory.Name).ToList() ?? new List<string>(),
                Creators = course.Creators.Select(c => c.User.ToString()).ToList()
            };
        }
    }
} 