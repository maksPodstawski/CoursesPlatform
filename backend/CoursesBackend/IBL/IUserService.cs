using Model;

namespace IBL
{
    public interface IUserService
    {
        int GetUserCountAsync();
        IQueryable<User> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByEmailAsync(string email);  
        IQueryable<User> GetUsersByFirstNameAsync(string firstName);  
        IQueryable<User> GetUsersByLastNameAsync(string lastName);  
        IQueryable<User> GetUsersByCourseIdAsync(Guid courseId);  


        Task<User> AddUserAsync(User user);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeleteUserAsync(Guid userId);
    }
}
