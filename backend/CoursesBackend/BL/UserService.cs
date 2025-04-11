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
        public async Task<int> GetUserCountAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Count();
        }
        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return _userRepository.GetUsersAsync();
        }
        public Task<User?> GetUserByIdAsync(Guid userId)
        {
            return _userRepository.GetUserByIDAsync(userId);
        }
        public Task<User?> GetUserByEmailAsync(string email)  
        {
            return _userRepository.GetUserByEmailAsync(email);
        }
        public Task<IEnumerable<User>> GetUsersByFirstNameAsync(string firstName)  
        {
            return _userRepository.GetUsersByFirstNameAsync(firstName);
        }
        public Task<IEnumerable<User>> GetUsersByLastNameAsync(string lastName) 
        {
            return _userRepository.GetUsersByLastNameAsync(lastName);
        }
        public Task<IEnumerable<User>> GetUsersByCourseIdAsync(Guid courseId) 
        {
            return _userRepository.GetUsersByCourseIdAsync(courseId);
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
