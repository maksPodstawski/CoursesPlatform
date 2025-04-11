using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;


namespace DAL
{
    public class CourseRepository : ICourseRepository
    {
        private readonly CoursesPlatformContext _context;

        public CourseRepository(CoursesPlatformContext context)
        {
            _context = context;
        }
        
        public async Task<Course?> GetCourseByIDAsync(Guid courseID)
        {
            return await _context.Courses.FindAsync(courseID);
        }
        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title)  
        {
            return await _context.Courses
                .Where(c => c.Name.Contains(title))
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByPriceAsync(decimal price)  
        {
            return await _context.Courses
                .Where(c => c.Price == price)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByAverageRatingAsync(double rating)  
        {
            return await _context.Courses
                .Where(c => c.Reviews != null && c.Reviews.Average(r => r.Rating) >= rating)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByCreatorAsync(Guid creatorId)  
        {
            return await _context.Creators
                .Where(c => c.UserId == creatorId)  
                .Select(c => c.Course)  
                .ToListAsync();
        }



        public async Task AddCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCourseAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCourseAsync(Guid courseID)
        {
            var course = await _context.Courses.FindAsync(courseID);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}
