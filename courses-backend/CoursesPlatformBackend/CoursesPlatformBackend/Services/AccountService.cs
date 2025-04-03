using CoursesPlatformBackend.DTO;
using CoursesPlatformBackend.Interfaces;
using CoursesPlatformBackend.Model;
using Microsoft.AspNetCore.Identity;

namespace CoursesPlatformBackend.Services
{
    public class AccountService : IAccountService
    {
        private readonly IJwtService jwtService;
        private readonly UserManager<User> userManager;
        private readonly IUserRepository userRepository;
        public AccountService(IJwtService jwtService, UserManager<User> userManager, IUserRepository userRepository)
        {
            this.jwtService = jwtService;
            this.userManager = userManager;
            this.userRepository = userRepository;
        }

        public async Task RegisterAsync(RegisterRequestDTO registerRequest)
        {
            var userExists = await userManager.FindByEmailAsync(registerRequest.Email) != null;

            if (userExists)
            {
                throw new Exception("User with this email already exists");
            }

            var user = User.Create(registerRequest.Email, registerRequest.FirstName, registerRequest.LastName);
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, registerRequest.Password);

            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new Exception("Registration Failed");
            }
        }

        public async Task LoginAsync(LoginRequestDTO loginRequest)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                throw new Exception("Login Failed");
            }

            var (jwtToken, expirationDateInUtc) = jwtService.GenerateJwtToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await userManager.UpdateAsync(user);

            jwtService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            jwtService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);

        }

        public async Task RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new Exception("Refreshing token is missing.");
            }

            var user = await userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if(user == null)
            {
                throw new Exception("Unable to retrieve user for refresh token.");
            }

            if(user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                throw new Exception("Refresh token is expired.");
            }

            var (jwtToken, expirationDateInUtc) = jwtService.GenerateJwtToken(user);
            var refreshTokenValue = jwtService.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

            await userManager.UpdateAsync(user);

            jwtService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            jwtService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }
    }
}
