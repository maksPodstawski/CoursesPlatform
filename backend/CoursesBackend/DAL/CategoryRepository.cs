using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CoursesPlatformContext _context;

        public CategoryRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Category?> GetCategoryByIDAsync(Guid categoryID)
        {
            return await _context.Categories.FindAsync(categoryID);
        }
        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<IEnumerable<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Subcategories
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
        }



        public async Task AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategoryAsync(Guid categoryID)
        {
            var category = await GetCategoryByIDAsync(categoryID);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
