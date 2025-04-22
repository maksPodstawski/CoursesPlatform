using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CourseSubcategory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        [Required]
        public Guid SubcategoryId { get; set; }
        [ForeignKey(nameof(SubcategoryId))]
        public Subcategory Subcategory { get; set; }


    }
}
