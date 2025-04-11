using Model;

namespace IBL
{
    public interface IUserService
    {
        Task<int> GetUserCountAsync();
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);  
        Task<IEnumerable<User>> GetUsersByFirstNameAsync(string firstName);  
        Task<IEnumerable<User>> GetUsersByLastNameAsync(string lastName);  
        Task<IEnumerable<User>> GetUsersByCourseIdAsync(Guid courseId);  


        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(Guid userId);
    }
}
