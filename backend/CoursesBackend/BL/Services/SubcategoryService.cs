using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public SubcategoryService(ISubcategoryRepository subcategoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
        }

        public async Task<List<Subcategory>> GetAllSubcategoriesAsync()
        {
            return await _subcategoryRepository.GetSubcategories().ToListAsync();
        }

        public async Task<Subcategory?> GetSubcategoryByIdAsync(Guid subcategoryId)
        {
            return await Task.FromResult(_subcategoryRepository.GetSubcategoryByID(subcategoryId));
        }

        public async Task<List<Subcategory>> GetSubcategoriesByCategoryIdAsync(Guid categoryId)
        {
            return await _subcategoryRepository.GetSubcategories()
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Subcategory> AddSubcategoryAsync(Subcategory subcategory)
        {
            subcategory.Id = Guid.NewGuid();
            return await Task.FromResult(_subcategoryRepository.AddSubcategory(subcategory));
        }

        public async Task<Subcategory?> UpdateSubcategoryAsync(Subcategory subcategory)
        {
            return await Task.FromResult(_subcategoryRepository.UpdateSubcategory(subcategory));
        }

        public async Task<Subcategory?> DeleteSubcategoryAsync(Guid subcategoryId)
        {
            return await Task.FromResult(_subcategoryRepository.DeleteSubcategory(subcategoryId));
        }
    }
}
