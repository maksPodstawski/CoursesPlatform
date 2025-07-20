using BL.Exceptions;
using DAL;
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
        private readonly ICategoryRepository _categoryRepository;

        public SubcategoryService(ISubcategoryRepository subcategoryRepository, ICategoryRepository categoryRepository)
        {
            _subcategoryRepository = subcategoryRepository;
            _categoryRepository = categoryRepository;
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
            if (subcategory == null)
                throw new ArgumentNullException(nameof(subcategory));

            if (string.IsNullOrWhiteSpace(subcategory.Name))
                throw new ArgumentException("Subcategory name cannot be null or empty.", nameof(subcategory.Name));

            var exists = await _subcategoryRepository.GetSubcategories()
                .AnyAsync(s => s.Name.ToLower() == subcategory.Name.ToLower() && s.CategoryId == subcategory.CategoryId);

            if (exists)
                throw new SubcategoryAlreadyExistsException(subcategory.Name);

            var category = _categoryRepository.GetCategoryById(subcategory.CategoryId);
            if (category != null && category.Name.ToLower() == subcategory.Name.ToLower())
                throw new ArgumentException("Subcategory name cannot be the same as category name.", nameof(subcategory.Name));

            subcategory.Id = Guid.NewGuid();
            return await Task.FromResult(_subcategoryRepository.AddSubcategory(subcategory));
        }

        public async Task<Subcategory?> UpdateSubcategoryAsync(Subcategory subcategory)
        {
            if (subcategory == null)
                throw new ArgumentNullException(nameof(subcategory));

            if (string.IsNullOrWhiteSpace(subcategory.Name))
                throw new ArgumentException("Subcategory name cannot be null or empty.", nameof(subcategory.Name));

            var exists = await _subcategoryRepository.GetSubcategories()
                .AnyAsync(s => s.Name.ToLower() == subcategory.Name.ToLower() && s.CategoryId == subcategory.CategoryId && s.Id != subcategory.Id);

            if (exists)
                throw new SubcategoryAlreadyExistsException(subcategory.Name);

            var category = _categoryRepository.GetCategoryById(subcategory.CategoryId);
            if (category != null && category.Name.ToLower() == subcategory.Name.ToLower())
                throw new ArgumentException("Subcategory name cannot be the same as category name.", nameof(subcategory.Name));

            return await Task.FromResult(_subcategoryRepository.UpdateSubcategory(subcategory));
        }

        public async Task<Subcategory?> DeleteSubcategoryAsync(Guid subcategoryId)
        {
            return await Task.FromResult(_subcategoryRepository.DeleteSubcategory(subcategoryId));
        }

        public async Task<Subcategory?> GetSubcategoryByNameAsync(string name, Guid categoryId)
        {
            return await _subcategoryRepository.GetSubcategories()
                .FirstOrDefaultAsync(s =>
                    s.Name.ToLower() == name.ToLower() &&
                    s.CategoryId == categoryId
                );
        }

    }
}
