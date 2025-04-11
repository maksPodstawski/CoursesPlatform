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
        public void AddUser(User user)
        {
            _userRepository.InsertUser(user);
            _userRepository.Save();
        }
        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
            _userRepository.Save();
        }
        public void DeleteUser(Guid id)
        {
            _userRepository.DeleteUser(id);
            _userRepository.Save();
        }
    }
}
