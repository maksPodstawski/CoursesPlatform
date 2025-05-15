using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class UserRepository : IUserRepository
    {

        private readonly CoursesPlatformContext _context;

        public UserRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public User? GetUserByRefreshToken(string refreshToken)
        {
            return _context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        }

        public IQueryable<User> GetUsers()
        {
            return _context.Users;
        }
        public User? GetUserByID(Guid userID)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userID);
        }

        public User AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
        public User? UpdateUser(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser == null)
                return null;

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;

            _context.SaveChanges();
            return existingUser;
        }
        public User? DeleteUser(Guid userID)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return null;

            _context.Users.Remove(user);
            _context.SaveChanges();
            return user;
        }

    }
}
