using IBL;
using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public SubcategoryService(ISubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
        }

        public  IQueryable<Subcategory> GetAllSubcategoriesAsync()
        {
            return  _subcategoryRepository.GetSubcategories();
        }

        public async Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId)
        {
            return await _subcategoryRepository.GetSubcategoryByIDAsync(subcategoryId);
        }

        public IQueryable<Subcategory> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            var all =  _subcategoryRepository.GetSubcategories();
            return all.Where(s => s.CategoryId == categoryId);
        }

        public async Task AddSubcategoryAsync(Subcategory subcategory)
        {
            subcategory.Id = Guid.NewGuid();
            await _subcategoryRepository.AddSubcategoryAsync(subcategory);
        }

        public async Task UpdateSubcategoryAsync(Subcategory subcategory)
        {
            await _subcategoryRepository.UpdateSubcategoryAsync(subcategory);
        }

        public async Task DeleteSubcategoryAsync(Guid subcategoryId)
        {
            await _subcategoryRepository.DeleteSubcategoryAsync(subcategoryId);
        }
    }
}
