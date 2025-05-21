using System;
using System.ComponentModel.DataAnnotations;
using Model;

namespace Model.DTO
{
    public class CreateStageDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public double Duration { get; set; }

        [Required]
        public Guid CourseId { get; set; }
    }

    public class UpdateStageDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public double Duration { get; set; }
    }

    public class StageResponseDTO
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Duration { get; set; }
        public string? VideoPath { get; set; }
        public DateTime CreatedAt { get; set; }

        public static StageResponseDTO FromStage(Stage stage)
        {
            return new StageResponseDTO
            {
                Id = stage.Id,
                CourseId = stage.CourseId,
                Name = stage.Name,
                Description = stage.Description,
                Duration = stage.Duration,
                VideoPath = stage.VideoPath,
                CreatedAt = stage.CreatedAt
            };
        }
    }
} 