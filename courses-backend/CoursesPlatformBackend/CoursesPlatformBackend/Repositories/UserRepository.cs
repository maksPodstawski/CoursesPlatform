using CoursesPlatformBackend.Data;
using CoursesPlatformBackend.Interfaces;
using CoursesPlatformBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace CoursesPlatformBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PlatformDbContext platformDbContext;

        public UserRepository(PlatformDbContext platformDbContext)
        {
            this.platformDbContext = platformDbContext;
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            var user = await platformDbContext.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

            return user;
        }
    }
}
