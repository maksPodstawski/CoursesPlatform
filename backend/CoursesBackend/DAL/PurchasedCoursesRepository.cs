using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class PurchasedCoursesRepository : IPurchasedCoursesRepository
    {
        private readonly CoursesPlatformContext _context;

        public PurchasedCoursesRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public IQueryable<PurchasedCourses> GetPurchasedCourses()
        {
            return _context.PurchasedCourses;
        }
        public PurchasedCourses? GetPurchasedCourseByID(Guid purchasedCourseID)
        {
            return _context.PurchasedCourses.FirstOrDefault(pc => pc.Id == purchasedCourseID);
        }
        public PurchasedCourses AddPurchasedCourse(PurchasedCourses purchasedCourse)
        {
            _context.PurchasedCourses.Add(purchasedCourse);
            _context.SaveChanges();
            return purchasedCourse;
        }
        public PurchasedCourses? UpdatePurchasedCourse(PurchasedCourses purchasedCourse)
        {
            var existing = _context.PurchasedCourses.FirstOrDefault(pc => pc.Id == purchasedCourse.Id);
            if (existing == null)
                return null;

            existing.UserId = purchasedCourse.UserId;
            existing.CourseId = purchasedCourse.CourseId;
            existing.PurchasedAt = purchasedCourse.PurchasedAt;
            existing.PurchasedPrice = purchasedCourse.PurchasedPrice;
            existing.ExpirationDate = purchasedCourse.ExpirationDate;
            existing.IsActive = purchasedCourse.IsActive;

            _context.SaveChanges();
            return existing;
        }
        public PurchasedCourses? DeletePurchasedCourse(Guid purchasedCourseID)
        {
            var existing = _context.PurchasedCourses.FirstOrDefault(pc => pc.Id == purchasedCourseID);
            if (existing == null)
                return null;

            _context.PurchasedCourses.Remove(existing);
            _context.SaveChanges();
            return existing;
        }
    }
}
