using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class UserRepository : IUserRepository
    {

        private CoursesPlatformContext _context;

        public UserRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public void DeleteUser(Guid userID)
        {
            User user = _context.Users.Find(userID);
            _context.Users.Remove(user);

        }
        public User GetUserByID(Guid userID)
        {
            return _context.Users.Find(userID);
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public void InsertUser(User user)
        {
            _context.Users.Add(user);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
