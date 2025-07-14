using BL.Exceptions;
using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Moq;


namespace CONTROLLERS.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _mockService;
        private readonly Mock<IRecaptchaService> _mockRecaptcha;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockService = new Mock<IAccountService>();
            _mockRecaptcha = new Mock<IRecaptchaService>();
            _controller = new AccountController(_mockService.Object, _mockRecaptcha.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            var request = new RegisterRequestDTO
            {
                RecaptchaToken = "token123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "StrongP@ssw0rd",
                ConfirmPassword = "StrongP@ssw0rd"
            };

            _mockRecaptcha.Setup(s => s.VerifyAsync("token123"))
                .ReturnsAsync(true);

            _mockService.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDTO>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.Register(request);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ReturnsBadRequest()
        {
            var request = new RegisterRequestDTO
            {
                RecaptchaToken = "token123",
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockRecaptcha.Setup(s => s.VerifyAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockService.Setup(s => s.RegisterAsync(request))
                .ThrowsAsync(new UserAlreadyExistsException("Email already exists"));

            var result = await _controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorObj = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);
            Assert.True(errorObj.ContainsKey("Email"));
            var errorMessages = (string[])errorObj["Email"];
            Assert.Contains("already exists", errorMessages[0]);
        }

        [Fact]
        public async Task Register_RegistrationFailed_ReturnsBadRequest()
        {
            var request = new RegisterRequestDTO
            {
                RecaptchaToken = "token123",
                FirstName = "John",
                LastName = "Doe",
                Email = "newuser@example.com",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            var rawErrors = new[] { "Password too weak", "Password too short" };

            _mockRecaptcha.Setup(s => s.VerifyAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockService.Setup(s => s.RegisterAsync(request))
                .ThrowsAsync(new RegistrationFailedException(rawErrors));

            var result = await _controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errorObj = Assert.IsAssignableFrom<SerializableError>(badRequest.Value);

            Assert.True(errorObj.ContainsKey("Password"));

            var errorMessages = (string[])errorObj["Password"];

            foreach (var expected in rawErrors)
            {
                Assert.Contains(expected, errorMessages);
            }
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsOk()
        {
            var request = new LoginRequestDTO
            {
                Email = "john@example.com",
                Password = "StrongP@ssw0rd"
            };

            var result = await _controller.Login(request);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Login_LoginFailed_ReturnsBadRequest()
        {
            var request = new LoginRequestDTO
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            };

            _mockService.Setup(s => s.LoginAsync(request))
                .ThrowsAsync(new LoginFailedException());

            var result = await _controller.Login(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<SerializableError>(badRequest.Value);
            Assert.True(errors.ContainsKey("Login"));
            Assert.Contains("Invalid email or password.", (string[])errors["Login"]);
        }

        [Fact]
        public async Task RefreshToken_WithTokenInCookies_CallsServiceAndReturnsOk()
        {
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["REFRESH_TOKEN"]).Returns("token-value");

            var context = new DefaultHttpContext();
            context.Request.Cookies = cookiesMock.Object;
            _controller.ControllerContext.HttpContext = context;

            var result = await _controller.RefreshToken();

            _mockService.Verify(s => s.RefreshTokenAsync("token-value"), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task GetMe_Authorized_ReturnsOkWithUser()
        {
            var userDTO = new UserDTO { FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            _mockService.Setup(s => s.GetMeAsync()).ReturnsAsync(userDTO);

            var result = await _controller.GetMe();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userDTO, okResult.Value);
        }

        [Fact]
        public async Task GetMe_UnauthorizedAccess_ReturnsUnauthorized()
        {
            _mockService.Setup(s => s.GetMeAsync())
                .ThrowsAsync(new UnauthorizedAccessException("Not authorized"));

            var result = await _controller.GetMe();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Not authorized", unauthorized.Value);
        }

        [Fact]
        public async Task GetMe_UnexpectedException_ReturnsInternalServerError()
        {
            _mockService.Setup(s => s.GetMeAsync())
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.GetMe();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Unexpected error", objectResult.Value);
        }

        [Fact]
        public async Task Logout_CallsServiceAndReturnsOk()
        {
            var result = await _controller.Logout();

            _mockService.Verify(s => s.LogoutAsync(), Times.Once);
            Assert.IsType<OkResult>(result);
        }
    }
}
