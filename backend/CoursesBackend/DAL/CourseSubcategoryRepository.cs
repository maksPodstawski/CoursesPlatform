using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class CourseSubcategoryRepository : ICourseSubcategoryRepository
    {
        private readonly CoursesPlatformContext _context;

        public CourseSubcategoryRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public IQueryable<CourseSubcategory> GetCourseSubcategories()
        {
            return _context.CourseSubcategories;
        }
        public CourseSubcategory? GetCourseSubcategoryByID(Guid courseSubcategoryId)
        {
            return _context.CourseSubcategories.FirstOrDefault(c => c.Id == courseSubcategoryId);
        }


        public CourseSubcategory AddCourseSubcategory(CourseSubcategory courseSubcategory)
        {
            _context.CourseSubcategories.Add(courseSubcategory);
            _context.SaveChanges();
            return courseSubcategory;
        }
        public CourseSubcategory? UpdateCourseSubcategory(CourseSubcategory courseSubcategory)
        {
            var existing = _context.CourseSubcategories.FirstOrDefault(c => c.Id == courseSubcategory.Id);
            if (existing == null)
                return null;

            existing.CourseId = courseSubcategory.CourseId;
            existing.SubcategoryId = courseSubcategory.SubcategoryId;

            _context.SaveChanges();
            return existing;
        }
        public CourseSubcategory? DeleteCourseSubcategory(Guid courseSubcategoryId)
        {
            var subcategory = _context.CourseSubcategories.FirstOrDefault(c => c.Id == courseSubcategoryId);
            if (subcategory == null)
                return null;

            _context.CourseSubcategories.Remove(subcategory);
            _context.SaveChanges();
            return subcategory;
        }
    }
}
