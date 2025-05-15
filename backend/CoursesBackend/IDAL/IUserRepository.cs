using Model;

namespace IDAL
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsers();
        User? GetUserByID(Guid userID);
        User AddUser(User user);
        User? UpdateUser(User user);
        User? DeleteUser(Guid userID);
        User? GetUserByRefreshToken(string refreshToken);
    }
}
