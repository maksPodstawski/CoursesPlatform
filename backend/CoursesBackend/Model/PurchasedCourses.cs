using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("PurchasedCourses")]
    public class PurchasedCourses
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        public decimal PurchasedPrice { get; set; } = 0.0M;
        public DateTime? ExpirationDate { get; set; } = null;
        public bool IsActive { get; set; } = true;
    }
}
