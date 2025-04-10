using IBL;
using IDAL;

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
    }
}
