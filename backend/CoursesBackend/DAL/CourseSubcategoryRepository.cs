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

        public async Task<IEnumerable<CourseSubcategory>> GetCourseSubcategoriesAsync()
        {
            return await _context.CourseSubcategories.ToListAsync();
        }

        public async Task<CourseSubcategory?> GetCourseSubcategoryByIDAsync(Guid courseSubcategoryID)
        {
            return await _context.CourseSubcategories.FindAsync(courseSubcategoryID);
        }

        public async Task AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            await _context.CourseSubcategories.AddAsync(courseSubcategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            _context.Entry(courseSubcategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseSubcategoryAsync(Guid courseSubcategoryID)
        {
            var courseSubcategory = await GetCourseSubcategoryByIDAsync(courseSubcategoryID);
            if (courseSubcategory != null)
            {
                _context.CourseSubcategories.Remove(courseSubcategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
