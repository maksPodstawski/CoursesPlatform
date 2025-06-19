using BL.Exceptions;
using BL.Services;
using IBL;
using IDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Constans;
using Model.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BL.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IAuthTokenService> _authTokenServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IRecaptchaService> _recaptchaServiceMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _userManagerMock = MockUserManager();
            _authTokenServiceMock = new Mock<IAuthTokenService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _recaptchaServiceMock = new Mock<IRecaptchaService>();

            _accountService = new AccountService(
                _authTokenServiceMock.Object,
                _userManagerMock.Object,
                _userRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _recaptchaServiceMock.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WhenUserDoesNotExist_ShouldRegisterUser()
        {
            var dto = new RegisterRequestDTO
            {
                Email = "test@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!",
                FirstName = "John",
                LastName = "Doe",
                RecaptchaToken = "dummy-token"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                            .ReturnsAsync((User)null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), IdentityRoleConstants.User))
                            .ReturnsAsync(IdentityResult.Success);

            await _accountService.RegisterAsync(dto);

            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
            _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), IdentityRoleConstants.User), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenUserExists_ShouldThrowException()
        {
            var dto = new RegisterRequestDTO
            {
                Email = "existing@example.com",
                Password = "Pass123!",
                ConfirmPassword = "Pass123!",
                FirstName = "Jane",
                LastName = "Doe",
                RecaptchaToken = "dummy-token"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                            .ReturnsAsync(new User());

            await Assert.ThrowsAsync<UserAlreadyExistsException>(() => _accountService.RegisterAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WithCorrectCredentials_ShouldGenerateTokens()
        {
            var dto = new LoginRequestDTO { Email = "test@example.com", Password = "Pass123!" };
            var user = new User { Email = dto.Email };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
                            .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password))
                            .ReturnsAsync(true);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                            .ReturnsAsync(new List<string> { "User" });

            _authTokenServiceMock.Setup(x => x.GenerateJwtToken(user, It.IsAny<IList<string>>()))
                                 .Returns(("jwt_token", DateTime.UtcNow.AddMinutes(5)));

            _authTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                                 .Returns("refresh_token");

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                            .ReturnsAsync(IdentityResult.Success);

            await _accountService.LoginAsync(dto);

            _authTokenServiceMock.Verify(x => x.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            _authTokenServiceMock.Verify(x => x.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserDoesNotExist()
        {
            var dto = new LoginRequestDTO { Email = "nonexistent@example.com", Password = "test123" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<LoginFailedException>(() => _accountService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
        {
            var user = new User { Email = "test@example.com" };
            var dto = new LoginRequestDTO { Email = user.Email, Password = "wrongpassword" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            await Assert.ThrowsAsync<LoginFailedException>(() => _accountService.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceed_AndSetTokensCorrectly()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
            var dto = new LoginRequestDTO { Email = user.Email, Password = "correct" };

            var roles = new List<string> { "User" };
            var accessToken = "jwt_token";
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(15);
            var refreshToken = "refresh_token";

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _authTokenServiceMock.Setup(x => x.GenerateJwtToken(user, roles)).Returns((accessToken, accessTokenExpiry));
            _authTokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            await _accountService.LoginAsync(dto);

            Assert.Equal(refreshToken, user.RefreshToken);
            Assert.True(user.RefreshTokenExpiresAtUtc > DateTime.UtcNow);
            _authTokenServiceMock.Verify(x => x.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", accessToken, accessTokenExpiry), Times.Once);
            _authTokenServiceMock.Verify(x => x.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_WithValidToken_ShouldGenerateNewTokens()
        {
            var user = new User
            {
                RefreshToken = "valid_token",
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(1)
            };

            _userRepositoryMock.Setup(x => x.GetUserByRefreshToken("valid_token"))
                               .Returns(user);

            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                            .ReturnsAsync(new List<string> { "User" });

            _authTokenServiceMock.Setup(x => x.GenerateJwtToken(user, It.IsAny<IList<string>>()))
                                 .Returns(("jwt_token", DateTime.UtcNow.AddMinutes(5)));

            _authTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                                 .Returns("new_refresh_token");

            _userManagerMock.Setup(x => x.UpdateAsync(user))
                            .ReturnsAsync(IdentityResult.Success);

            await _accountService.RefreshTokenAsync("valid_token");

            Assert.Equal("new_refresh_token", user.RefreshToken);
            _authTokenServiceMock.Verify(x => x.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldClearTokens_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            var httpContext = new DefaultHttpContext
            {
                User = userClaims
            };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var user = new User
            {
                Id = userId,
                RefreshToken = "old_token",
                RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(1)
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            await _accountService.LogoutAsync();

            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiresAtUtc);
            _authTokenServiceMock.Verify(x => x.ClearAuthTokenCookie("ACCESS_TOKEN"), Times.Once);
            _authTokenServiceMock.Verify(x => x.ClearAuthTokenCookie("REFRESH_TOKEN"), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldNotThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            var httpContext = new DefaultHttpContext
            {
                User = userClaims
            };

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

            var exception = await Record.ExceptionAsync(() => _accountService.LogoutAsync());
            Assert.Null(exception);
            _authTokenServiceMock.Verify(x => x.ClearAuthTokenCookie("ACCESS_TOKEN"), Times.Once);
            _authTokenServiceMock.Verify(x => x.ClearAuthTokenCookie("REFRESH_TOKEN"), Times.Once);
        }

        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, new PasswordHasher<User>(), 
                null, null, null, null, null, null);
        }
    }

}