using IBL;
using IDAL;
using Model;

namespace BL
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public int GetUserCountAsync()
        {
            var users =  _userRepository.GetUsers();
            return users.Count();
        }
        public IQueryable<User> GetAllUsersAsync()
        {
            return _userRepository.GetUsers();
        }
        public Task<User?> GetUserByIdAsync(Guid userId)
        {
            return _userRepository.GetUserByIDAsync(userId);
        }
        public Task<User?> GetUserByEmailAsync(string email)  
        {
            return _userRepository.GetUserByEmailAsync(email);
        }
        public IQueryable<User> GetUsersByFirstNameAsync(string firstName)
        {
            var users =  _userRepository.GetUsers();
            return users.Where(u => u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase));
        }
        public IQueryable<User> GetUsersByLastNameAsync(string lastName)
        {
            var users =  _userRepository.GetUsers();
            return users.Where(u => u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
        }
        public IQueryable<User> GetUsersByCourseIdAsync(Guid courseId)
        {
            var users =  _userRepository.GetUsers();
            return users.Where(u => u.Reviews != null && u.Reviews.Any(r => r.CourseId == courseId));
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
            return user;
        }
        public async Task<User?> UpdateUserAsync(User user)
        {
            var existing = await _userRepository.GetUserByIDAsync(user.Id);
            if (existing == null)
                return null;

            await _userRepository.UpdateUserAsync(user);
            return user;
        }
        public async Task<User?> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIDAsync(userId);
            if (user == null)
                return null;

            await _userRepository.DeleteUserAsync(userId);
            return user;
        }
    }
}
