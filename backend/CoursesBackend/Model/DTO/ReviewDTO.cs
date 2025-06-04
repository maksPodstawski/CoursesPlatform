using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public record CreateReviewDTO
    {
        [Required]
        public required Guid CourseId { get; init; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; init; }

        [Required]
        [MaxLength(250)]
        public required string Comment { get; init; }
    }

    public record UpdateReviewDTO
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; init; }

        [Required]
        [MaxLength(250)]
        public required string Comment { get; init; }
    }

    public record ReviewResponseDTO
    {
        public Guid Id { get; init; }
        public int Rating { get; init; }
        public string Comment { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public string UserName { get; init; } = string.Empty;
        public Guid CourseId { get; init; }
        public string CourseName { get; init; } = string.Empty;

        public static ReviewResponseDTO FromReview(Review review)
        {
            return new ReviewResponseDTO
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UserName = review.User.ToString(),
                CourseId = review.CourseId,
                CourseName = review.Course.Name
            };
        }

        public record DeleteReviewsDto
        {
            public List<Guid> ReviewIds { get; set; }
        }
    }
} 