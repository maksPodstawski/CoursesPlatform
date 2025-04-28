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

        public async Task<PurchasedCourses?> GetPurchasedCourseByIDAsync(Guid purchasedCourseID)
        {
            return await _context.PurchasedCourses.FindAsync(purchasedCourseID);
        }

        public async Task AddPurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            await _context.PurchasedCourses.AddAsync(purchasedCourse);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            _context.Entry(purchasedCourse).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletePurchasedCourseAsync(Guid purchasedCourseID)
        {
            var purchasedCourse = await GetPurchasedCourseByIDAsync(purchasedCourseID);
            if (purchasedCourse != null)
            {
                _context.PurchasedCourses.Remove(purchasedCourse);
                await _context.SaveChangesAsync();
            }
        }
    }
}
