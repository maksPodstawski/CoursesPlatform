using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;


namespace DAL
{
    public class CourseRepository : ICourseRepository
    {
        private CoursesPlatformContext _context;

        public CourseRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public void DeleteCourse(Guid courseID)
        {
            Course course = _context.Courses.Find(courseID);
            _context.Courses.Remove(course);
        }

        public Course GetCourseByID(Guid courseID)
        {
            return _context.Courses.Find(courseID);
        }

        public IEnumerable<Course> GetCourses()
        {
            return _context.Courses.ToList();
        }

        public void InsertCourse(Course course)
        {
            _context.Courses.Add(course);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateCourse(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
        }

    }
}
