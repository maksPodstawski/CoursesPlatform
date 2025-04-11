using Model;

namespace IBL
{
    public interface IUserService
    {
        int GetUserCount();
        IEnumerable<User> GetAllUsers();
        User? GetUserById(Guid id);
        User AddUser(User user);
        User? UpdateUser(User user);
        User? DeleteUser(Guid id);
    }
}
