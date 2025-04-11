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

        public async Task DeleteCourseAsync(Guid courseID)
        {
            var course = await _context.Courses.FindAsync(courseID);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Course?> GetCourseByIDAsync(Guid courseID)
        {
            return await _context.Courses.FindAsync(courseID);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task InsertCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

    }
}
