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

        public IQueryable<Category> GetCategories()
        {
            return _context.Categories;
        }
        public Category? GetCategoryById(Guid categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        }
        public Category AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category? UpdateCategory(Category category)
        {
            var existing = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing == null)
                return null;

            existing.Name = category.Name;
            _context.SaveChanges();
            return existing;
        }

        public Category? DeleteCategory(Guid categoryId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
                return null;

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return category;
        }
    }
}
