using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Subcategories")]
    public class Subcategory
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public List<CourseSubcategory>? Courses { get; set; } = new List<CourseSubcategory>();
    }
}
