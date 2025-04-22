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
            return _context.CourseSubcategories.AsQueryable();
        }

        public async Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid courseSubcategoryId)
        {
            return await _context.CourseSubcategories.FindAsync(courseSubcategoryId);
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

        public async Task DeleteCourseSubcategoryAsync(Guid courseSubcategoryId)
        {
            var courseSubcategory = await GetCourseSubcategoryByIdAsync(courseSubcategoryId);
            if (courseSubcategory != null)
            {
                _context.CourseSubcategories.Remove(courseSubcategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}
