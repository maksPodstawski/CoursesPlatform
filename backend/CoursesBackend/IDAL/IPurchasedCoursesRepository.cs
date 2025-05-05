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
        IQueryable<PurchasedCourses> GetPurchasedCourses();
        PurchasedCourses? GetPurchasedCourseByID(Guid purchasedCourseID);
        PurchasedCourses AddPurchasedCourse(PurchasedCourses purchasedCourse);
        PurchasedCourses? UpdatePurchasedCourse(PurchasedCourses purchasedCourse);
        PurchasedCourses? DeletePurchasedCourse(Guid purchasedCourseID);
    }
}
