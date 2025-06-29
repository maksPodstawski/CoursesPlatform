using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public class PurchaseCourseDTO
    {
        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public DateTime? ExpirationDate { get; set; }

    }

    public class PurchaseCourseResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public decimal PurchasedPrice { get; set; }
        public DateTime PurchasedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public static PurchaseCourseResponseDTO FromPurchasedCourse(Model.PurchasedCourses purchase)
        {
            return new PurchaseCourseResponseDTO
            {
                Id = purchase.Id,
                UserId = purchase.UserId,
                CourseId = purchase.CourseId,
                PurchasedPrice = purchase.PurchasedPrice,
                PurchasedAt = purchase.PurchasedAt,
                ExpirationDate = purchase.ExpirationDate,
                IsActive = purchase.IsActive
            };
        }


    }
} 