using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IPurchasedCoursesRepository
    {
        Task<IEnumerable<PurchasedCourses>> GetPurchasedCoursesAsync();
        Task<PurchasedCourses?> GetPurchasedCourseByIDAsync(Guid purchasedCourseID);
        Task AddPurchasedCourseAsync(PurchasedCourses purchasedCourse);
        Task DeletePurchasedCourseAsync(Guid purchasedCourseID);
        Task UpdatePurchasedCourseAsync(PurchasedCourses purchasedCourse);
    }
}
