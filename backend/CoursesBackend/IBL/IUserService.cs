using Model;

namespace IBL
{
    public interface IUserService
    {
        int GetUserCount();
        IEnumerable<User> GetAllUsers();
        User? GetUserById(Guid id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(Guid id);
    }
}
