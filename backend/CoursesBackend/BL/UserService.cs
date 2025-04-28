using DAL;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL
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
            return await Task.Run(() => _userRepository.GetUsers().Count());
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.Run(() => _userRepository.GetUsers().ToList());
        }
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await Task.Run(() => _userRepository.GetUserByID(userId));
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await Task.Run(() => _userRepository.GetUsers()
                .FirstOrDefault(u => u.Email == email));
        }
        public async Task<List<User>> GetUsersByFirstNameAsync(string firstName)
        {
            return await Task.Run(() => _userRepository.GetUsers()
                .Where(u => u.FirstName == firstName)
                .ToList());
        }
        public async Task<List<User>> GetUsersByLastNameAsync(string lastName)
        {
            return await Task.Run(() => _userRepository.GetUsers()
                .Where(u => u.LastName == lastName)
                .ToList());
        }
        public async Task<List<User>> GetUsersByCourseIdAsync(Guid courseId)
        {
            return await Task.Run(() =>
                _purchasedCoursesRepository.GetPurchasedCourses()
                    .Where(pc => pc.CourseId == courseId)
                    .Select(pc => pc.User)
                    .Distinct()
                    .ToList()
            );
        }



        public async Task<User> AddUserAsync(User user)
        {
            return await Task.Run(() => _userRepository.AddUser(user));
        }
        public async Task<User?> UpdateUserAsync(User user)
        {
            return await Task.Run(() => _userRepository.UpdateUser(user));
        }
        public async Task<User?> DeleteUserAsync(Guid userId)
        {
            return await Task.Run(() => _userRepository.DeleteUser(userId));
        }
    }
}
