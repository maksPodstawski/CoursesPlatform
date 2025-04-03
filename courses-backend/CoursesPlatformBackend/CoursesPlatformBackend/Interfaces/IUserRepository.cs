using CoursesPlatformBackend.Data;
using CoursesPlatformBackend.Model;

namespace CoursesPlatformBackend.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    }
}
