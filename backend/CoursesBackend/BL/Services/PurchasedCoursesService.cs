using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class PurchasedCoursesService : IPurchasedCoursesService
    {
        private readonly IPurchasedCoursesRepository _purchasedCoursesRepository;
        public PurchasedCoursesService(IPurchasedCoursesRepository purchasedCoursesRepository)
        {
            _purchasedCoursesRepository = purchasedCoursesRepository;
        }


        public async Task<List<PurchasedCourses>> GetAllPurchasedCoursesAsync()
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses().ToListAsync();
        }
        public async Task<PurchasedCourses?> GetPurchasedCourseByIdAsync(Guid id)
        {
            return await Task.FromResult(_purchasedCoursesRepository.GetPurchasedCourseByID(id));
        }
        public async Task<List<PurchasedCourses>> GetPurchasedCoursesByUserIdAsync(Guid userId)
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.UserId == userId)
                .ToListAsync();
        }
        public async Task<List<PurchasedCourses>> GetPurchasedCoursesByCourseIdAsync(Guid courseId)
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<List<PurchasedCourses>> GetActivePurchasedCoursesAsync()
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.IsActive)
                .ToListAsync();
        }
        public async Task<List<PurchasedCourses>> GetExpiredPurchasedCoursesAsync()
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.ExpirationDate.HasValue && pc.ExpirationDate < DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task<List<PurchasedCourses>> GetActiveCoursesByUserSortedAsync(Guid userId)
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.UserId == userId && pc.IsActive)
                .OrderByDescending(pc => pc.PurchasedAt)
                .ToListAsync();
        }
        public async Task<bool> HasUserPurchasedCourseAsync(Guid userId, Guid courseId)
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .AnyAsync(pc => pc.UserId == userId && pc.CourseId == courseId);
        }



        public async Task<PurchasedCourses> AddPurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            return await Task.FromResult(_purchasedCoursesRepository.AddPurchasedCourse(purchasedCourse));
        }
        public async Task<PurchasedCourses?> UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            return await Task.FromResult(_purchasedCoursesRepository.UpdatePurchasedCourse(purchasedCourse));
        }
        public async Task<PurchasedCourses?> DeletePurchasedCourseAsync(Guid id)
        {
            return await Task.FromResult(_purchasedCoursesRepository.DeletePurchasedCourse(id));
        }
        public async Task<int> GetPurchaseCountByCourseIdAsync(Guid courseId)
        {
            var purchases = await GetPurchasedCoursesByCourseIdAsync(courseId);
            return purchases.Select(p => p.UserId).Distinct().Count();
        }

    }
}
