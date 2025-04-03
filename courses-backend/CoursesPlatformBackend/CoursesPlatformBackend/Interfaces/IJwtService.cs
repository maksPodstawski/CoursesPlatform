using CoursesPlatformBackend.Model;

namespace CoursesPlatformBackend.Interfaces
{
    public interface IJwtService
    {
        public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user);
        public string GenerateRefreshToken();
        public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
    }
}
