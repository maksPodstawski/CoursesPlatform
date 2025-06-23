using Model;

namespace IBL
{
    public interface IPurchasedCoursesService
    {
        Task<List<PurchasedCourses>> GetAllPurchasedCoursesAsync();
        Task<PurchasedCourses?> GetPurchasedCourseByIdAsync(Guid id);
        Task<List<PurchasedCourses>> GetPurchasedCoursesByUserIdAsync(Guid userId);
        Task<List<PurchasedCourses>> GetPurchasedCoursesByCourseIdAsync(Guid courseId);
        Task<List<PurchasedCourses>> GetActivePurchasedCoursesAsync();
        Task<List<PurchasedCourses>> GetExpiredPurchasedCoursesAsync();
        Task<List<PurchasedCourses>> GetActiveCoursesByUserSortedAsync(Guid userId);
        Task<bool> HasUserPurchasedCourseAsync(Guid userId, Guid courseId);


        Task<PurchasedCourses> AddPurchasedCourseAsync(PurchasedCourses purchasedCourse);
        Task<PurchasedCourses?> UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse);
        Task<PurchasedCourses?> DeletePurchasedCourseAsync(Guid id);
        Task<int> GetPurchaseCountByCourseIdAsync(Guid courseId);

    }
}
