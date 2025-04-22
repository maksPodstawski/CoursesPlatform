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
        
        public async Task<Course?> GetCourseByIdAsync(Guid courseId)
        {
            return await _context.Courses.FindAsync(courseId);
        }
        public IQueryable<Course> GetCourses()
        {
            return _context.Courses.AsQueryable();
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
        public async Task DeleteCourseAsync(Guid courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}
