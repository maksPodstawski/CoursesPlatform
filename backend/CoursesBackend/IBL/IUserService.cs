using Model;

namespace IBL
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetUsersByFirstNameAsync(string firstName);
        Task<List<User>> GetUsersByLastNameAsync(string lastName);
        Task<List<User>> GetUsersByCourseIdAsync(Guid courseId);

        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(Guid userId);
    }
}
