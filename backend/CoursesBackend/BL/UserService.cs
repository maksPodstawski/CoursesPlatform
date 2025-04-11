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
        public Task<User?> GetUserByIdAsync(Guid id)
        {
            return _userRepository.GetUserByIDAsync(id);
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
        public async Task<User?> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIDAsync(id);
            if (user == null)
                return null;

            await _userRepository.DeleteUserAsync(id);
            return user;
        }
    }
}
