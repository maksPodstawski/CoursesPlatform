using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Stages")]
    public class Stage
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [MaxLength(250)]
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public double Duration { get; set; }
        public string? VideoPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
