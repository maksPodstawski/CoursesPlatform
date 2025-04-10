using Model;

namespace IDAL
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUserByID(Guid userID);
        void InsertUser(User user);
        void DeleteUser(Guid userID);
        void UpdateUser(User user);
        void Save();
    }
}
