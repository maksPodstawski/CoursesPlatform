using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BL.Options;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model;

namespace BL.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthTokenService(IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.ToString()),

            };

            var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationTimeInMinutes);

            var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return (jwtToken, expires);
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
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                cookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expiration,
                    IsEssential = true,
                });
        }
    }
}
