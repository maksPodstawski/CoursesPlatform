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
        public int GetUserCount()
        {
            return _userRepository.GetUsers().Count();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetUsers();
        }
        public User? GetUserById(Guid id)
        {
            return _userRepository.GetUserByID(id);
        }
        public User AddUser(User user)
        {
            _userRepository.InsertUser(user);
            _userRepository.Save();
            return user;
        }
        public User? UpdateUser(User user)
        {
            var existing = _userRepository.GetUserByID(user.Id);
            if (existing == null) return null;

            _userRepository.UpdateUser(user);
            _userRepository.Save();
            return user;
        }
        public User? DeleteUser(Guid id)
        {
            var user = _userRepository.GetUserByID(id);
            if (user == null) return null;

            _userRepository.DeleteUser(id);
            _userRepository.Save();
            return user;
        }
    }
}
