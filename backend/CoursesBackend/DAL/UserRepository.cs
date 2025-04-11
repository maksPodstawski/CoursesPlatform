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

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIDAsync(Guid userID)
        {
            return await _context.Users.FindAsync(userID);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<IEnumerable<User>> GetUsersByFirstNameAsync(string firstName)
        {
            return await _context.Users
                .Where(u => u.FirstName.Contains(firstName))
                .ToListAsync();
        }
        public async Task<IEnumerable<User>> GetUsersByLastNameAsync(string lastName)
        {
            return await _context.Users
                .Where(u => u.LastName.Contains(lastName))
                .ToListAsync();
        }
        public async Task<IEnumerable<User>> GetUsersByCourseIdAsync(Guid courseId)
        {
            return await _context.PurchasedCourses
                .Where(pc => pc.CourseId == courseId)
                .Select(pc => pc.User)  
                .ToListAsync();
        }
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid userID)
        {
            var user = await GetUserByIDAsync(userID);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
