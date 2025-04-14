using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly CoursesPlatformContext _context;

        public SubcategoryRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subcategory>> GetSubcategoriesAsync()
        {
            return await _context.Subcategories.ToListAsync();
        }

        public async Task<Subcategory?> GetSubcategoryByIDAsync(Guid subcategoryID)
        {
            return await _context.Subcategories.FindAsync(subcategoryID);
        }

        public async Task AddSubcategoryAsync(Subcategory subcategory)
        {
            await _context.Subcategories.AddAsync(subcategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSubcategoryAsync(Subcategory subcategory)
        {
            _context.Subcategories.Update(subcategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubcategoryAsync(Guid subcategoryID)
        {
            var subcategory = await GetSubcategoryByIDAsync(subcategoryID);
            if (subcategory != null)
            {
                _context.Subcategories.Remove(subcategory);
                await _context.SaveChangesAsync();
            }
        }

        public Task InsertSubcategoryAsync(Subcategory subcategory)
        {
            throw new NotImplementedException();
        }
    }
}
