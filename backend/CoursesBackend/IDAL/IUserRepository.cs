using Model;

namespace IDAL
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIDAsync(Guid userID);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByFirstNameAsync(string firstName);
        Task<IEnumerable<User>> GetUsersByLastNameAsync(string lastName);
        Task<IEnumerable<User>> GetUsersByCourseIdAsync(Guid courseId);


        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid userID);
    }
}
