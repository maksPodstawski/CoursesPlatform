using DAL;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPurchasedCoursesRepository _purchasedCoursesRepository;

        public UserService(IUserRepository userRepository, IPurchasedCoursesRepository purchasedCoursesRepository)
        {
            _userRepository = userRepository;
            _purchasedCoursesRepository = purchasedCoursesRepository;
        }

        public async Task<int> GetUserCountAsync()
        {
            return (await _userRepository.GetUsers().ToListAsync()).Count;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetUsers().ToListAsync();
        }
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await Task.FromResult(_userRepository.GetUserByID(userId));
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUsers().FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<List<User>> GetUsersByFirstNameAsync(string firstName)
        {
            return await _userRepository.GetUsers()
                .Where(u => u.FirstName == firstName)
                .ToListAsync();
        }
        public async Task<List<User>> GetUsersByLastNameAsync(string lastName)
        {
            return await _userRepository.GetUsers()
                .Where(u => u.LastName == lastName)
                .ToListAsync();
        }
        public async Task<List<User>> GetUsersByCourseIdAsync(Guid courseId)
        {
            return await _purchasedCoursesRepository.GetPurchasedCourses()
                .Where(pc => pc.CourseId == courseId)
                .Select(pc => pc.User)
                .Distinct()
                .ToListAsync();
        }


        public async Task<User> AddUserAsync(User user)
        {
            return await Task.FromResult(_userRepository.AddUser(user));
        }
        public async Task<User?> UpdateUserAsync(User user)
        {
            return await Task.FromResult(_userRepository.UpdateUser(user));
        }
        public async Task<User?> DeleteUserAsync(Guid userId)
        {
            return await Task.FromResult(_userRepository.DeleteUser(userId));
        }
    }
}
