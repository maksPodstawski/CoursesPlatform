using IBL;
using IDAL;
using Model;

namespace BL
{
    public class PurchasedCoursesService : IPurchasedCoursesService
    {
        private readonly IPurchasedCoursesRepository _purchasedCoursesRepository;
        public PurchasedCoursesService(IPurchasedCoursesRepository purchasedCoursesRepository)
        {
            _purchasedCoursesRepository = purchasedCoursesRepository;
        }

        public IQueryable<PurchasedCourses> GetAllPurchasedCoursesAsync()
        {
            return _purchasedCoursesRepository.GetPurchasedCourses();
        }
        public Task<PurchasedCourses?> GetPurchasedCourseByIdAsync(Guid id)
        {
            return _purchasedCoursesRepository.GetPurchasedCourseByIDAsync(id);
        }
        public async Task<PurchasedCourses> AddPurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            await _purchasedCoursesRepository.AddPurchasedCourseAsync(purchasedCourse);
            return purchasedCourse;
        }
        public async Task<PurchasedCourses?> UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse)
        {
            var existing = await _purchasedCoursesRepository.GetPurchasedCourseByIDAsync(purchasedCourse.Id);
            if (existing == null)
                return null;

            await _purchasedCoursesRepository.UpdatePurchasedCourseAsync(purchasedCourse);
            return purchasedCourse;
        }
        public async Task<PurchasedCourses?> DeletePurchasedCourseAsync(Guid id)
        {
            var existing = await _purchasedCoursesRepository.GetPurchasedCourseByIDAsync(id);
            if (existing == null)
                return null;

            await _purchasedCoursesRepository.DeletePurchasedCourseAsync(id);
            return existing;
        }
    }
}
