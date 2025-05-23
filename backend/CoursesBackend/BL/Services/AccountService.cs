using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BL.Exceptions;
using IBL;
using IDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Constans;
using Model.DTO;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenService _authTokenService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IAuthTokenService authTokenService, UserManager<User> userManager, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _authTokenService = authTokenService;
            _userManager = userManager;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            var userExists = await _userManager.FindByEmailAsync(registerRequestDTO.Email) != null;

            if (userExists)
            {
                throw new UserAlreadyExistsException(email: registerRequestDTO.Email);
            }

            var user = new User
            {
                Email = registerRequestDTO.Email,
                UserName = registerRequestDTO.Email,
                FirstName = registerRequestDTO.FirstName,
                LastName = registerRequestDTO.LastName
            };
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerRequestDTO.Password);

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new RegistrationFailedException(result.Errors.Select(x => x.Description));
            }

            await _userManager.AddToRoleAsync(user, IdentityRoleConstants.User);
        }

        public async Task LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password))
            {
                throw new LoginFailedException(email: loginRequestDTO.Email);
            }


            IList<string> roles = await _userManager.GetRolesAsync(user);

            var (jwtToken, expirationDateInUtc) = _authTokenService.GenerateJwtToken(user, roles);
            var refreshTokenValue = _authTokenService.GenerateRefreshToken();

            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7); //move to appsettings

            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;


            await _userManager.UpdateAsync(user);

            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }

        public async Task RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidRefreshTokenException("Refresh token is missing.");
            }
            var user = _userRepository.GetUserByRefreshToken(refreshToken);

            if(user == null)
            {
                throw new InvalidRefreshTokenException("Unable to retrive user for refresh token.");
            }
            
            if(user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException("Refresh token is expired.");
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            var (jwtToken, expirationDateInUtc) = _authTokenService.GenerateJwtToken(user, roles);
            var refreshTokenValue = _authTokenService.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7); //move to appsettings
            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;
            await _userManager.UpdateAsync(user);
            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }

        public async Task<UserDTO> GetMeAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                throw new UnauthorizedAccessException("User is not authenticated or invalid user ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task LogoutAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiresAtUtc = null;
                    await _userManager.UpdateAsync(user);
                }
            }

            _authTokenService.ClearAuthTokenCookie("ACCESS_TOKEN");
            _authTokenService.ClearAuthTokenCookie("REFRESH_TOKEN");
        }
    }
}
