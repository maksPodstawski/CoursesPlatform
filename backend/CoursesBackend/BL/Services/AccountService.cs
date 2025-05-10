using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Exceptions;
using IBL;
using IDAL;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.DTO;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenService _authTokenService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public AccountService(IAuthTokenService authTokenService, UserManager<User> userManager, IUserRepository userRepository)
        {
            _authTokenService = authTokenService;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            var userExists = await _userManager.FindByEmailAsync(registerRequestDTO.Email) != null;

            if (userExists)
            {
                throw new UserAlredyExistsException(email: registerRequestDTO.Email);
            }

            var user = User.Create(registerRequestDTO.Email, registerRequestDTO.FirstName, registerRequestDTO.LastName);
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerRequestDTO.Password);

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new RegistrationFailedException(result.Errors.Select(x => x.Description));
            }
        }

        public async Task LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password))
            {
                throw new LoginFailedException(email: loginRequestDTO.Email);
            }
            var (jwtToken, expirationDateInUtc) = _authTokenService.GenerateJwtToken(user);
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

            var (jwtToken, expirationDateInUtc) = _authTokenService.GenerateJwtToken(user);
            var refreshTokenValue = _authTokenService.GenerateRefreshToken();
            var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7); //move to appsettings
            user.RefreshToken = refreshTokenValue;
            user.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;
            await _userManager.UpdateAsync(user);
            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
            _authTokenService.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", user.RefreshToken, refreshTokenExpirationDateInUtc);
        }
    }
}
