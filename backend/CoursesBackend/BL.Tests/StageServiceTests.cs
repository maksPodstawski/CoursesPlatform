using BL.Exceptions;
using BL.Services;
using IDAL;
using MockQueryable.Moq;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Tests
{
    public class StageServiceTests
    {
        private readonly Mock<IStageRepository> _mockStageRepository;
        private readonly StageService _stageService;

        public StageServiceTests()
        {
            _mockStageRepository = new Mock<IStageRepository>();
            _stageService = new StageService(_mockStageRepository.Object);
        }

        [Fact]
        public async Task GetStageByIdAsync_ExistingId_ReturnsStage()
        {
            var stageId = Guid.NewGuid();
            var stage = new Stage
            {
                Id = stageId,
                Name = "Stage 1",
                Description = "Test description"
            };

            _mockStageRepository.Setup(r => r.GetStageById(stageId)).Returns(stage);

            var result = await _stageService.GetStageByIdAsync(stageId);

            Assert.NotNull(result);
            Assert.Equal(stageId, result.Id);
            Assert.Equal("Stage 1", result.Name);
        }

        [Fact]
        public async Task GetStageByIdAsync_NonExistingId_ReturnsNull()
        {
            var stageId = Guid.NewGuid();
            _mockStageRepository.Setup(r => r.GetStageById(stageId)).Returns((Stage)null);

            var result = await _stageService.GetStageByIdAsync(stageId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllStagesAsync_ReturnsAllStages()
        {
            var stages = new List<Stage>
            {
                new Stage { Id = Guid.NewGuid(), Name = "Stage 1", Description = "Desc 1" },
                new Stage { Id = Guid.NewGuid(), Name = "Stage 2", Description = "Desc 2" }
            };

            var mockDbSet = stages.AsQueryable().BuildMockDbSet();
            _mockStageRepository.Setup(r => r.GetStages()).Returns(mockDbSet.Object);

            var result = await _stageService.GetAllStagesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Name == "Stage 1");
            Assert.Contains(result, s => s.Name == "Stage 2");
        }

        [Fact]
        public async Task AddStageAsync_AddsAndReturnsStage()
        {
            var stage = new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "New Stage",
                Description = "New Description"
            };

            var stages = new List<Stage>().AsQueryable();
            var mockDbSet = stages.BuildMockDbSet();
            _mockStageRepository.Setup(r => r.GetStages()).Returns(mockDbSet.Object);

            _mockStageRepository.Setup(r => r.AddStage(It.IsAny<Stage>())).Returns(stage);

            var result = await _stageService.AddStageAsync(stage);

            Assert.Equal(stage.Id, result.Id);
            Assert.Equal(stage.Name, result.Name);
            _mockStageRepository.Verify(r => r.AddStage(stage), Times.Once);
        }

        [Fact]
        public async Task UpdateStageAsync_UpdatesAndReturnsStage()
        {
            var stage = new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "Updated Stage",
                Description = "Updated Description"
            };

            var stages = new List<Stage>().AsQueryable();
            var mockDbSet = stages.BuildMockDbSet();
            _mockStageRepository.Setup(r => r.GetStages()).Returns(mockDbSet.Object);

            _mockStageRepository.Setup(r => r.UpdateStage(It.IsAny<Stage>())).Returns(stage);

            var result = await _stageService.UpdateStageAsync(stage);

            Assert.NotNull(result);
            Assert.Equal("Updated Stage", result.Name);
            _mockStageRepository.Verify(r => r.UpdateStage(stage), Times.Once);
        }

        [Fact]
        public async Task DeleteStageAsync_ExistingId_DeletesAndReturnsStage()
        {
            var stageId = Guid.NewGuid();
            var stage = new Stage { Id = stageId, Name = "To Delete", Description = "Desc" };

            _mockStageRepository.Setup(r => r.DeleteStage(stageId)).Returns(stage);

            var result = await _stageService.DeleteStageAsync(stageId);

            Assert.NotNull(result);
            Assert.Equal(stageId, result.Id);
            _mockStageRepository.Verify(r => r.DeleteStage(stageId), Times.Once);
        }

        [Fact]
        public async Task DeleteStageAsync_NonExistingId_ReturnsNull()
        {
            var stageId = Guid.NewGuid();
            _mockStageRepository.Setup(r => r.DeleteStage(stageId)).Returns((Stage)null);

            var result = await _stageService.DeleteStageAsync(stageId);

            Assert.Null(result);
            _mockStageRepository.Verify(r => r.DeleteStage(stageId), Times.Once);
        }

        [Fact]
        public async Task AddStageAsync_StageAlreadyExists_ThrowsStageAlreadyExistsInCourseException()
        {
            var stage = new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "Duplicate Stage",
                Description = "Desc"
            };

            var stages = new List<Stage>
            {
                new Stage { Id = Guid.NewGuid(), CourseId = stage.CourseId, Name = "Duplicate Stage", Description = "Other" }
            };

            var mockDbSet = stages.AsQueryable().BuildMockDbSet();
            _mockStageRepository.Setup(r => r.GetStages()).Returns(mockDbSet.Object);

            var ex = await Assert.ThrowsAsync<StageAlreadyExistsInCourseException>(() => _stageService.AddStageAsync(stage));
            Assert.Contains("Duplicate Stage", ex.Message);
        }
    }
}
