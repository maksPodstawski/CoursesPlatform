using Model;

namespace IBL
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(Guid id);
    }
}
