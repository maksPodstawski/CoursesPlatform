using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoursesPlatformBackend.Interfaces;
using CoursesPlatformBackend.Model;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;


namespace CoursesPlatformBackend.Service
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtService(IHttpContextAccessor httpContextAccessor) 
        {
             this.httpContextAccessor = httpContextAccessor;
        }

        public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user)
        {
            var envVars = DotEnv.Read();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envVars["JWT_SECRET"]));

            var cridentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ToString())
            };

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(envVars["JWT_EXPIRATION_IN_MINUTES"]));

            var token = new JwtSecurityToken(
                issuer: envVars["JWT_ISSUER"],
                audience: envVars["JWT_AUDIENCE"],
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: cridentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtToken, expiresAtUtc);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration)
        {
            this.httpContextAccessor.HttpContext.Response.Cookies.Append(
                cookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expiration,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    Secure = true   // if false for http
                }
            );
        }
    }
}
