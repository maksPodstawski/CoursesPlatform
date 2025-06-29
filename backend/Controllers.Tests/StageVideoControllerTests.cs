using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Controllers.Tests
{
    public class StageVideoControllerTests
    {
        private readonly Mock<IStageService> _mockStageService;
        private readonly Mock<IPurchasedCoursesService> _mockPurchasedCoursesService;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;
        private readonly Mock<ICreatorService> _mockCreatorService;
        private readonly StageVideoController _controller;

        public StageVideoControllerTests()
        {
            _mockStageService = new Mock<IStageService>();
            _mockPurchasedCoursesService = new Mock<IPurchasedCoursesService>();
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockCreatorService = new Mock<ICreatorService>();

            _controller = new StageVideoController(
                _mockStageService.Object,
                _mockPurchasedCoursesService.Object,
                _mockEnvironment.Object,
                _mockCreatorService.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
        }

        private void SetUser(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims);
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task UploadVideo_NoFile_ReturnsBadRequest()
        {
            SetUser(Guid.NewGuid().ToString());

            var result = await _controller.UploadVideo(Guid.NewGuid(), null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded", badRequest.Value);
        }

        [Fact]
        public async Task UploadVideo_InvalidContentType_ReturnsBadRequest()
        {
            SetUser(Guid.NewGuid().ToString());

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");

            var result = await _controller.UploadVideo(Guid.NewGuid(), fileMock.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Only video files are allowed", badRequest.Value);
        }

        [Fact]
        public async Task UploadVideo_StageNotFound_ReturnsNotFound()
        {
            SetUser(Guid.NewGuid().ToString());

            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Stage)null);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.ContentType).Returns("video/mp4");

            var result = await _controller.UploadVideo(Guid.NewGuid(), fileMock.Object);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Stage not found", notFound.Value);
        }

        [Fact]
        public async Task UploadVideo_UserNotAuthenticated_ReturnsUnauthorized()
        {
            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Stage());

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.ContentType).Returns("video/mp4");

            var result = await _controller.UploadVideo(Guid.NewGuid(), fileMock.Object);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task UploadVideo_UserNotCreator_ReturnsForbid()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var stage = new Stage { Id = Guid.NewGuid(), CourseId = Guid.NewGuid() };

            _mockStageService.Setup(s => s.GetStageByIdAsync(stage.Id)).ReturnsAsync(stage);
            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(userId, stage.CourseId)).ReturnsAsync(false);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.ContentType).Returns("video/mp4");

            var result = await _controller.UploadVideo(stage.Id, fileMock.Object);

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UploadVideo_Success_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var stage = new Stage { Id = Guid.NewGuid(), CourseId = Guid.NewGuid() };

            _mockStageService.Setup(s => s.GetStageByIdAsync(stage.Id)).ReturnsAsync(stage);
            _mockCreatorService.Setup(s => s.IsUserCreatorOfCourseAsync(userId, stage.CourseId)).ReturnsAsync(true);
            _mockStageService.Setup(s => s.UpdateStageAsync(It.IsAny<Stage>())).ReturnsAsync(stage);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.ContentType).Returns("video/mp4");
            fileMock.Setup(f => f.FileName).Returns("video.mp4");
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var result = await _controller.UploadVideo(stage.Id, fileMock.Object);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Video uploaded successfully", ok.Value.ToString());
        }

        [Fact]
        public async Task StreamVideo_StageNotFound_ReturnsNotFound()
        {
            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Stage)null);

            var result = await _controller.StreamVideo(Guid.NewGuid());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Stage not found", notFound.Value);
        }

        [Fact]
        public async Task StreamVideo_NoVideoPath_ReturnsNotFound()
        {
            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new Stage { VideoPath = null });

            var result = await _controller.StreamVideo(Guid.NewGuid());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No video found for this stage", notFound.Value);
        }

        [Fact]
        public async Task StreamVideo_UserNotAuthenticated_ReturnsUnauthorized()
        {
            var stage = new Stage { VideoPath = "path/to/video.mp4" };
            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync(stage);

            var result = await _controller.StreamVideo(Guid.NewGuid());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task StreamVideo_UserNotPurchased_ReturnsForbid()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var stage = new Stage { CourseId = Guid.NewGuid(), VideoPath = "path/to/video.mp4" };
            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync(stage);
            _mockPurchasedCoursesService.Setup(s => s.HasUserPurchasedCourseAsync(userId, stage.CourseId)).ReturnsAsync(false);

            var result = await _controller.StreamVideo(Guid.NewGuid());

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task StreamVideo_FileNotFound_ReturnsNotFound()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var stage = new Stage { CourseId = Guid.NewGuid(), VideoPath = "nonexistent.mp4" };

            _mockStageService.Setup(s => s.GetStageByIdAsync(It.IsAny<Guid>())).ReturnsAsync(stage);
            _mockPurchasedCoursesService.Setup(s => s.HasUserPurchasedCourseAsync(userId, stage.CourseId)).ReturnsAsync(true);

            var result = await _controller.StreamVideo(Guid.NewGuid());

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Video file not found", notFound.Value);
        }
    }
}
