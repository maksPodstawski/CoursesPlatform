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

        public IQueryable<Subcategory> GetSubcategories()
        {
            return _context.Subcategories;
        }
        public Subcategory? GetSubcategoryByID(Guid subcategoryID)
        {
            return _context.Subcategories.FirstOrDefault(s => s.Id == subcategoryID);
        }

        public Subcategory AddSubcategory(Subcategory subcategory)
        {
            _context.Subcategories.Add(subcategory);
            _context.SaveChanges();
            return subcategory;
        }

        public Subcategory? UpdateSubcategory(Subcategory subcategory)
        {
            var existing = _context.Subcategories.FirstOrDefault(s => s.Id == subcategory.Id);
            if (existing == null)
                return null;

            _context.Subcategories.Update(subcategory);
            _context.SaveChanges();
            return subcategory;
        }

        public Subcategory? DeleteSubcategory(Guid subcategoryID)
        {
            var subcategory = _context.Subcategories.FirstOrDefault(s => s.Id == subcategoryID);
            if (subcategory == null)
                return null;

            _context.Subcategories.Remove(subcategory);
            _context.SaveChanges();
            return subcategory;
        }
    }
}
