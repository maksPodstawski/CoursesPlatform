using Model;

namespace IBL
{
    public interface IPurchasedCoursesService
    {
        IQueryable<PurchasedCourses> GetAllPurchasedCoursesAsync();
        Task<PurchasedCourses?> GetPurchasedCourseByIdAsync(Guid id);
        Task<PurchasedCourses> AddPurchasedCourseAsync(PurchasedCourses purchasedCourse);
        Task<PurchasedCourses?> UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse);
        Task<PurchasedCourses?> DeletePurchasedCourseAsync(Guid id);
    }
}
