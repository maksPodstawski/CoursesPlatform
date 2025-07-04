using COURSES.API.Controllers;
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
    public class StageControllerTests
    {
        private readonly Mock<IStageService> _mockStageService;
        private readonly Mock<ICreatorService> _mockCreatorService;
        private readonly StageController _controller;

        public StageControllerTests()
        {
            _mockStageService = new Mock<IStageService>();
            _mockCreatorService = new Mock<ICreatorService>();

            _controller = new StageController(_mockStageService.Object, _mockCreatorService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        private void SetUser(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims);
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task CreateStage_NullDto_ReturnsBadRequest()
        {
            var result = await _controller.CreateStage(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Stage data is required", badRequest.Value);
        }

        [Fact]
        public async Task CreateStage_UserNotLoggedIn_ReturnsUnauthorized()
        {
            var createDto = new CreateStageDTO
            {
                CourseId = Guid.NewGuid(),
                Name = "Test Stage",
                Description = "Test Description",
                Duration = 1.5
            };

            var result = await _controller.CreateStage(createDto);
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CreateStage_UserNotCreator_ReturnsForbid()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var createDto = new CreateStageDTO
            {
                CourseId = Guid.NewGuid(),
                Name = "Test Stage",
                Description = "Test Description",
                Duration = 1.5
            };

            _mockCreatorService
                .Setup(s => s.IsUserCreatorOfCourseAsync(Guid.Parse(userId), createDto.CourseId))
                .ReturnsAsync(false);

            var result = await _controller.CreateStage(createDto);

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CreateStage_StageServiceReturnsNull_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var createDto = new CreateStageDTO
            {
                CourseId = Guid.NewGuid(),
                Name = "Test Stage",
                Description = "Test Description",
                Duration = 1.5
            };

            _mockCreatorService
                .Setup(s => s.IsUserCreatorOfCourseAsync(Guid.Parse(userId), createDto.CourseId))
                .ReturnsAsync(true);

            _mockStageService
                .Setup(s => s.AddStageAsync(It.IsAny<Stage>()))
                .ReturnsAsync((Stage)null);

            var result = await _controller.CreateStage(createDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Failed to create stage", badRequest.Value);
        }

        [Fact]
        public async Task CreateStage_ValidData_ReturnsCreated()
        {
            var userId = Guid.NewGuid().ToString();
            SetUser(userId);

            var createDto = new CreateStageDTO
            {
                CourseId = Guid.NewGuid(),
                Name = "Test Stage",
                Description = "Test Description",
                Duration = 1.5
            };

            var createdStage = new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = createDto.CourseId,
                Name = createDto.Name,
                Description = createDto.Description,
                Duration = createDto.Duration,
                CreatedAt = DateTime.UtcNow
            };

            _mockCreatorService
                .Setup(s => s.IsUserCreatorOfCourseAsync(Guid.Parse(userId), createDto.CourseId))
                .ReturnsAsync(true);

            _mockStageService
                .Setup(s => s.AddStageAsync(It.IsAny<Stage>()))
                .ReturnsAsync(createdStage);


            var result = await _controller.CreateStage(createDto);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<StageResponseDTO>(createdAt.Value);
            Assert.Equal(createdStage.Id, returned.Id);
            Assert.Equal(createDto.Name, returned.Name);
        }
    }
}
