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
    [Table("Courses")]
    public class Course
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [MaxLength(250)]
        public string? Description { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public int Duration { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public ICollection<Stage>? Stages { get; set; } = new List<Stage>();
        public ICollection<CourseSubcategory>? CourseSubcategories { get; set; } = new List<CourseSubcategory>();
        public ICollection<Creator> Creators { get; set; } = new List<Creator>();
        public static Course FromCreateDTO(CreateCourseDTO dto)
        {
            return new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                Duration = dto.Duration,
                Price = dto.Price,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
