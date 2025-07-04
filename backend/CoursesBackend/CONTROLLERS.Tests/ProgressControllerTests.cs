using COURSES.API.Controllers;
using IBL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.DTOs;
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
    public class ProgressControllerTests
    {
        private readonly Mock<IProgressService> _mockProgressService;
        private readonly ProgressController _controller;

        public ProgressControllerTests()
        {
            _mockProgressService = new Mock<IProgressService>();
            _controller = new ProgressController(_mockProgressService.Object)
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
        public async Task GetUserProgress_ReturnsOkWithProgressList()
        {
            var userId = Guid.NewGuid();
            SetUser(userId.ToString());

            var progressList = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid(), UserId = userId, StageId = Guid.NewGuid() },
                new Progress { Id = Guid.NewGuid(), UserId = userId, StageId = Guid.NewGuid() }
            };

            _mockProgressService.Setup(s => s.GetProgressByUserIdAsync(userId)).ReturnsAsync(progressList);

            var result = await _controller.GetUserProgress();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<List<Progress>>(okResult.Value);
            Assert.Equal(progressList.Count, returnedList.Count);
            Assert.All(returnedList, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetStageProgress_ExistingProgress_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            SetUser(userId.ToString());

            var progressList = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid(), UserId = userId, StageId = stageId },
                new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = stageId }
            };

            _mockProgressService.Setup(s => s.GetProgressByStageIdAsync(stageId)).ReturnsAsync(progressList);

            var result = await _controller.GetStageProgress(stageId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var progress = Assert.IsType<Progress>(okResult.Value);
            Assert.Equal(userId, progress.UserId);
            Assert.Equal(stageId, progress.StageId);
        }

        [Fact]
        public async Task GetStageProgress_NoUserProgress_ReturnsOkWithNull()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            SetUser(userId.ToString());

            var progressList = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = stageId }
            };

            _mockProgressService.Setup(s => s.GetProgressByStageIdAsync(stageId)).ReturnsAsync(progressList);

            var result = await _controller.GetStageProgress(stageId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public async Task GetCourseStagesWithProgress_ReturnsOkWithList()
        {
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            SetUser(userId.ToString());

            var stagesWithProgress = new List<StageWithProgressDto>
            {
                new StageWithProgressDto { Id = Guid.NewGuid(), IsCompleted = false },
                new StageWithProgressDto { Id = Guid.NewGuid(), IsCompleted = true }
            };

            _mockProgressService.Setup(s => s.GetStagesWithProgressForCourseAsync(userId, courseId))
                .ReturnsAsync(stagesWithProgress);

            var result = await _controller.GetCourseStagesWithProgress(courseId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<List<StageWithProgressDto>>(okResult.Value);
            Assert.Equal(stagesWithProgress.Count, returnedList.Count);
        }

        [Fact]
        public async Task MarkStageAsCompleted_CallsServiceAndReturnsOk()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            SetUser(userId.ToString());

            _mockProgressService.Setup(s => s.MarkStageAsCompletedAsync(userId, stageId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _controller.MarkStageAsCompleted(stageId);

            Assert.IsType<OkResult>(result);
            _mockProgressService.Verify();
        }

        [Fact]
        public async Task StartStage_ExistingProgress_UpdatesAndReturnsDto()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            SetUser(userId.ToString());

            var existingDto = new ProgressDTO
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StageId = stageId,
                LastAccessedAt = new DateTime(2024, 1, 1),
                StartedAt = new DateTime(2023, 12, 31),
                IsCompleted = false
            };

            var updatedDto = new ProgressDTO
            {
                Id = existingDto.Id,
                UserId = userId,
                StageId = stageId,
                LastAccessedAt = new DateTime(2024, 1, 2),
                StartedAt = existingDto.StartedAt,
                IsCompleted = existingDto.IsCompleted
            };

            _mockProgressService.Setup(s => s.GetProgressByStageIdAsync(stageId))
                .ReturnsAsync(new List<Progress> { existingDto.ToEntity() });

            _mockProgressService.Setup(s => s.UpdateProgressAsync(It.IsAny<Progress>()))
                .ReturnsAsync(updatedDto.ToEntity());

            var result = await _controller.StartStage(stageId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ProgressDTO>(okResult.Value);

            Assert.Equal(updatedDto.Id, dto.Id);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal(stageId, dto.StageId);
            Assert.True(dto.LastAccessedAt > existingDto.LastAccessedAt);
        }

        [Fact]
        public async Task StartStage_NewProgress_AddsAndReturnsDto()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            SetUser(userId.ToString());

            _mockProgressService.Setup(s => s.GetProgressByStageIdAsync(stageId))
                .ReturnsAsync(new List<Progress>());

            Progress addedProgress = null;

            _mockProgressService.Setup(s => s.AddProgressAsync(It.IsAny<Progress>()))
                .Callback<Progress>(p => addedProgress = p)
                .ReturnsAsync((Progress p) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });

            var result = await _controller.StartStage(stageId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ProgressDTO>(okResult.Value);

            Assert.NotNull(addedProgress);
            Assert.Equal(userId, addedProgress.UserId);
            Assert.Equal(stageId, addedProgress.StageId);
            Assert.False(dto.IsCompleted);
            Assert.Equal(addedProgress.Id, dto.Id);
        }
    }
}
