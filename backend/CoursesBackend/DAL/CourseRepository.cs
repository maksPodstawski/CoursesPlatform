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


        public IQueryable<Course> GetCourses()
        {
            return _context.Courses
                .Include(c => c.Creators)
                .ThenInclude(creator => creator.User);
        }

        public Course? GetCourseById(Guid courseId)
        {
            return _context.Courses.FirstOrDefault(c => c.Id == courseId);
        }

        public Course AddCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return course;
        }
        public Course? UpdateCourse(Course course)
        {
            var existing = _context.Courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing == null)
                return null;

            existing.Name = course.Name;
            existing.Description = course.Description;
            existing.ImageUrl = course.ImageUrl;
            existing.Duration = course.Duration;
            existing.Price = course.Price;
            existing.Difficulty = course.Difficulty;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.SaveChanges();
            return existing;
        }
        public Course? DeleteCourse(Guid courseId)
        {
            var course = _context.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return null;

            _context.Courses.Remove(course);
            _context.SaveChanges();
            return course;
        }
    }
}
