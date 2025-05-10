using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class CourseSubcategoryService : ICourseSubcategoryService
    {
        private readonly ICourseSubcategoryRepository _courseSubcategoryRepository;
        public CourseSubcategoryService(ICourseSubcategoryRepository courseSubcategoryRepository)
        {
            _courseSubcategoryRepository = courseSubcategoryRepository;
        }


        public async Task<List<CourseSubcategory>> GetAllCourseSubcategoriesAsync()
        {
            return await _courseSubcategoryRepository.GetCourseSubcategories().ToListAsync();
        }
        public async Task<CourseSubcategory?> GetCourseSubcategoryByIdAsync(Guid id)
        {
            return await Task.FromResult(_courseSubcategoryRepository.GetCourseSubcategoryByID(id));
        }
        public async Task<List<CourseSubcategory>> GetByCourseIdAsync(Guid courseId)
        {
            return await _courseSubcategoryRepository.GetCourseSubcategories()
                .Where(cs => cs.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<List<CourseSubcategory>> GetBySubcategoryIdAsync(Guid subcategoryId)
        {
            return await _courseSubcategoryRepository.GetCourseSubcategories()
                .Where(cs => cs.SubcategoryId == subcategoryId)
                .ToListAsync();
        }



        public async Task<CourseSubcategory> AddCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            return await Task.FromResult(_courseSubcategoryRepository.AddCourseSubcategory(courseSubcategory));
        }
        public async Task<CourseSubcategory?> UpdateCourseSubcategoryAsync(CourseSubcategory courseSubcategory)
        {
            return await Task.FromResult(_courseSubcategoryRepository.UpdateCourseSubcategory(courseSubcategory));
        }
        public async Task<CourseSubcategory?> DeleteCourseSubcategoryAsync(Guid id)
        {
            return await Task.FromResult(_courseSubcategoryRepository.DeleteCourseSubcategory(id));
        }
    }
}
