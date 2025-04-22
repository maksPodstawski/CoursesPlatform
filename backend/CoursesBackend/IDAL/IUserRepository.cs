using Model;

namespace IDAL
{
    public interface IUserRepository
    {
        IQueryable<User> GetUsers();
        Task<User?> GetUserByIDAsync(Guid userID);
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid userID);
    }
}
