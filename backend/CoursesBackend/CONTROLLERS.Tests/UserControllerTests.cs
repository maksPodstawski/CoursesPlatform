using API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CONTROLLERS.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();

            _controller = new UserController(_mockUserService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private void SetUser(Guid userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var userPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = userPrincipal }
            };
        }

        #region GetMyProfile Tests

        [Fact]
        public async Task GetMyProfile_ValidUser_ReturnsUserProfile()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                UserName = "johndoe",
                PhoneNumber = "123456789",
                ProfilePicture = null
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var actionResult = await _controller.GetMyProfile();

            Assert.NotNull(actionResult);

            var resultObject = actionResult.Result ?? new OkObjectResult(actionResult.Value);

            var okResult = Assert.IsType<OkObjectResult>(resultObject);
            var dto = Assert.IsType<UserProfileDTO>(okResult.Value);

            Assert.Equal(user.FirstName, dto.FirstName);
            Assert.Equal(user.LastName, dto.LastName);
            Assert.Equal(user.Email, dto.Email);
            Assert.Equal(user.UserName, dto.UserName);
            Assert.Equal(user.PhoneNumber, dto.PhoneNumber);
            Assert.Null(dto.ProfilePictureBase64);
        }

        [Fact]
        public async Task GetMyProfile_UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetMyProfile();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMyProfile_NoUserIdClaim_ReturnsUnauthorized()
        {

            var result = await _controller.GetMyProfile();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetMyProfile_InvalidUserIdClaim_ReturnsUnauthorized()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "invalid-guid") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            var result = await _controller.GetMyProfile();

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        #endregion

        #region UpdateMyProfile Tests

        [Fact]
        public async Task UpdateMyProfile_ValidRequest_ReturnsNoContent()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var existingUser = new User
            {
                Id = userId,
                FirstName = "OldFirst",
                LastName = "OldLast",
                Email = "old@example.com",
                UserName = "oldusername",
                PhoneNumber = "000",
                ProfilePicture = null
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            var updateDto = new UserProfileDTO
            {
                FirstName = "NewFirst",
                LastName = "NewLast",
                Email = "new@example.com",
                UserName = "newusername",
                PhoneNumber = "111",
                ProfilePictureBase64 = Convert.ToBase64String(new byte[] { 10, 20, 30 })
            };

            var result = await _controller.UpdateMyProfile(updateDto);

            Assert.IsType<NoContentResult>(result);
            _mockUserService.Verify(s => s.UpdateUserAsync(It.Is<User>(u =>
                u.FirstName == updateDto.FirstName &&
                u.LastName == updateDto.LastName &&
                u.Email == updateDto.Email &&
                u.UserName == updateDto.UserName &&
                u.PhoneNumber == updateDto.PhoneNumber &&
                u.ProfilePicture != null
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateMyProfile_InvalidBase64_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var existingUser = new User { Id = userId };
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);

            var updateDto = new UserProfileDTO
            {
                ProfilePictureBase64 = "not-base64-data"
            };

            var result = await _controller.UpdateMyProfile(updateDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid image format.", badRequest.Value);
        }

        [Fact]
        public async Task UpdateMyProfile_UserNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.UpdateMyProfile(new UserProfileDTO());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateMyProfile_NoUserIdClaim_ReturnsUnauthorized()
        {

            var result = await _controller.UpdateMyProfile(new UserProfileDTO());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UpdateMyProfile_UpdateFails_ReturnsServerError()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var existingUser = new User { Id = userId };
            _mockUserService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockUserService.Setup(s => s.UpdateUserAsync(existingUser)).ReturnsAsync((User)null);

            var updateDto = new UserProfileDTO();

            var result = await _controller.UpdateMyProfile(updateDto);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Update failed", statusCodeResult.Value);
        }

        #endregion
    }
}
