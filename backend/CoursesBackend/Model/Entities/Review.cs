using Model.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public int Rating { get; set; }
        [MaxLength(250)]
        [Required]
        public string Comment { get; set; } = string.Empty;
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public static Review FromCreateDTO(CreateReviewDTO dto, Guid userId)
        {
            return new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                CourseId = dto.CourseId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }
        public void UpdateFromDTO(UpdateReviewDTO dto)
        {
            Rating = dto.Rating;
            Comment = dto.Comment;
        }
    }
}
