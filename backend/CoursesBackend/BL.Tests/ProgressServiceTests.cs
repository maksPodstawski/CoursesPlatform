using IDAL;
using Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MockQueryable.Moq;
using MockQueryable;
using Microsoft.EntityFrameworkCore;

namespace BL.Tests
{
    public class ProgressServiceTests
    {
        private readonly Mock<IProgressRepository> _mockProgressRepository;
        private readonly ProgressService _progressService;

        public ProgressServiceTests()
        {
            _mockProgressRepository = new Mock<IProgressRepository>();
            _progressService = new ProgressService(_mockProgressRepository.Object);
        }

        [Fact]
        public async Task GetAllProgressesAsync_ReturnsAllProgresses()
        {
            var progresses = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid() },
                new Progress { Id = Guid.NewGuid() }
            };

            var mockDbSet = progresses.AsQueryable().BuildMockDbSet();
            _mockProgressRepository.Setup(r => r.GetProgresses()).Returns(mockDbSet.Object);

            var result = await _progressService.GetAllProgressesAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProgressByIdAsync_ReturnsCorrectProgress()
        {
            var progressId = Guid.NewGuid();
            var progress = new Progress { Id = progressId };

            _mockProgressRepository.Setup(r => r.GetProgressById(progressId)).Returns(progress);

            var result = await _progressService.GetProgressByIdAsync(progressId);

            Assert.NotNull(result);
            Assert.Equal(progressId, result?.Id);
        }

        [Fact]
        public async Task GetProgressByUserIdAsync_ReturnsFilteredProgresses()
        {
            var userId = Guid.NewGuid();
            var progresses = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid(), UserId = userId },
                new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid() }
            };

            var mockDbSet = progresses.AsQueryable().BuildMockDbSet();
            _mockProgressRepository.Setup(r => r.GetProgresses()).Returns(mockDbSet.Object);

            var result = await _progressService.GetProgressByUserIdAsync(userId);

            Assert.Single(result);
            Assert.All(result, p => Assert.Equal(userId, p.UserId));
        }

        [Fact]
        public async Task GetProgressByStageIdAsync_ReturnsFilteredProgresses()
        {
            var stageId = Guid.NewGuid();
            var progresses = new List<Progress>
            {
                new Progress { Id = Guid.NewGuid(), StageId = stageId },
                new Progress { Id = Guid.NewGuid(), StageId = Guid.NewGuid() }
            };

            var mockDbSet = progresses.AsQueryable().BuildMockDbSet();
            _mockProgressRepository.Setup(r => r.GetProgresses()).Returns(mockDbSet.Object);

            var result = await _progressService.GetProgressByStageIdAsync(stageId);

            Assert.Single(result);
            Assert.All(result, p => Assert.Equal(stageId, p.StageId));
        }

        [Fact]
        public async Task AddProgressAsync_SetsFieldsAndAddsProgress()
        {
            var progress = new Progress { UserId = Guid.NewGuid(), StageId = Guid.NewGuid() };

            _mockProgressRepository.Setup(r => r.AddProgress(It.IsAny<Progress>())).Returns<Progress>(p => p);

            var result = await _progressService.AddProgressAsync(progress);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.True((DateTime.UtcNow - result.StartedAt).TotalSeconds < 5);
            Assert.True((DateTime.UtcNow - result.LastAccessedAt).TotalSeconds < 5);
        }

        [Fact]
        public async Task UpdateProgressAsync_UpdatesExistingProgress()
        {
            var progress = new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = Guid.NewGuid() };

            _mockProgressRepository.Setup(r => r.GetProgressById(progress.Id)).Returns(progress);
            _mockProgressRepository.Setup(r => r.UpdateProgress(progress)).Returns(progress);

            var result = await _progressService.UpdateProgressAsync(progress);

            Assert.NotNull(result);
            Assert.Equal(progress.Id, result?.Id);
        }

        [Fact]
        public async Task UpdateProgressAsync_ReturnsNull_WhenProgressNotFound()
        {
            var progress = new Progress { Id = Guid.NewGuid() };

            _mockProgressRepository.Setup(r => r.GetProgressById(progress.Id)).Returns((Progress?)null);

            var result = await _progressService.UpdateProgressAsync(progress);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProgressAsync_DeletesProgress_WhenExists()
        {
            var progressId = Guid.NewGuid();
            var progress = new Progress { Id = progressId };

            _mockProgressRepository.Setup(r => r.GetProgressById(progressId)).Returns(progress);
            _mockProgressRepository.Setup(r => r.DeleteProgress(progressId)).Returns(progress);

            var result = await _progressService.DeleteProgressAsync(progressId);

            Assert.NotNull(result);
            Assert.Equal(progressId, result?.Id);
        }

        [Fact]
        public async Task DeleteProgressAsync_ReturnsNull_WhenNotFound()
        {
            var progressId = Guid.NewGuid();
            _mockProgressRepository.Setup(r => r.GetProgressById(progressId)).Returns((Progress?)null);

            var result = await _progressService.DeleteProgressAsync(progressId);

            Assert.Null(result);
        }

        [Fact]
        public async Task MarkStageAsCompletedAsync_SetsIsCompleted_WhenNotCompleted()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            var progress = new Progress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StageId = stageId,
                IsCompleted = false
            };

            var progresses = new List<Progress> { progress };
            var mockDbSet = progresses.AsQueryable().BuildMockDbSet();
            _mockProgressRepository.Setup(r => r.GetProgresses()).Returns(mockDbSet.Object);
            _mockProgressRepository.Setup(r => r.AddProgress(It.IsAny<Progress>())).Returns<Progress>(p => p);

            await _progressService.MarkStageAsCompletedAsync(userId, stageId);

            Assert.True(progress.IsCompleted);
            Assert.NotNull(progress.CompletedAt);
        }

        [Fact]
        public async Task MarkStageAsCompletedAsync_DoesNothing_WhenAlreadyCompleted()
        {
            var userId = Guid.NewGuid();
            var stageId = Guid.NewGuid();
            var progress = new Progress
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StageId = stageId,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow.AddDays(-1)
            };

            var progresses = new List<Progress> { progress };
            var mockDbSet = progresses.AsQueryable().BuildMockDbSet();
            _mockProgressRepository.Setup(r => r.GetProgresses()).Returns(mockDbSet.Object);

            await _progressService.MarkStageAsCompletedAsync(userId, stageId);

            var previousDate = progress.CompletedAt;
            Assert.True(progress.IsCompleted);
            Assert.Equal(previousDate, progress.CompletedAt);
        }
        public class ProgressRepositoryDummy : IProgressRepository
        {
            public IQueryable<Progress> GetProgresses()
            {
                return new List<Progress>().AsQueryable(); 
            }

            public Progress? GetProgressById(Guid progressId)
            {
                return null; 
            }

            public Progress AddProgress(Progress progress)
            {
                return progress; 
            }

            public Progress? UpdateProgress(Progress progress)
            {
                return progress; 
            }

            public Progress? DeleteProgress(Guid progressId)
            {
                return null; 
            }
        }
        public class ProgressRepositoryStub : IProgressRepository
        {
            private readonly List<Progress> _progresses;

            public ProgressRepositoryStub(List<Progress> progresses)
            {
                _progresses = progresses;
            }

            public IQueryable<Progress> GetProgresses()
            {
                return _progresses.AsQueryable(); 
            }

            public Progress? GetProgressById(Guid progressId)
            {
                return _progresses.FirstOrDefault(p => p.Id == progressId); 
            }

            public Progress AddProgress(Progress progress)
            {
                _progresses.Add(progress);
                return progress; 
            }

            public Progress? UpdateProgress(Progress progress)
            {
                var existing = _progresses.FirstOrDefault(p => p.Id == progress.Id);
                if (existing != null)
                {
                    existing = progress; 
                    return existing;
                }
                return null;
            }

            public Progress? DeleteProgress(Guid progressId)
            {
                var progress = _progresses.FirstOrDefault(p => p.Id == progressId);
                if (progress != null)
                {
                    _progresses.Remove(progress);
                    return progress;
                }
                return null;
            }
        }
        public class ProgressRepositoryFake : IProgressRepository
        {
            private readonly List<Progress> _progresses = new List<Progress>();

            public IQueryable<Progress> GetProgresses()
            {
                return _progresses.AsQueryable(); 
            }

            public Progress? GetProgressById(Guid progressId)
            {
                return _progresses.FirstOrDefault(p => p.Id == progressId); 
            }

            public Progress AddProgress(Progress progress)
            {
                progress.Id = Guid.NewGuid(); 
                _progresses.Add(progress);
                return progress; 
            }

            public Progress? UpdateProgress(Progress progress)
            {
                var existing = _progresses.FirstOrDefault(p => p.Id == progress.Id);
                if (existing != null)
                {
                    _progresses.Remove(existing);
                    _progresses.Add(progress); 
                    return progress;
                }
                return null;
            }

            public Progress? DeleteProgress(Guid progressId)
            {
                var progress = _progresses.FirstOrDefault(p => p.Id == progressId);
                if (progress != null)
                {
                    _progresses.Remove(progress);
                    return progress; 
                }
                return null;
            }
        }
        public class ProgressRepositoryMock : IProgressRepository
        {
            private readonly Mock<IProgressRepository> _mock;

            public ProgressRepositoryMock()
            {
                _mock = new Mock<IProgressRepository>();
            }

            public IQueryable<Progress> GetProgresses()
            {
                return _mock.Object.GetProgresses();
            }

            public Progress? GetProgressById(Guid progressId)
            {
                return _mock.Object.GetProgressById(progressId);
            }

            public Progress AddProgress(Progress progress)
            {
                return _mock.Object.AddProgress(progress);
            }

            public Progress? UpdateProgress(Progress progress)
            {
                return _mock.Object.UpdateProgress(progress);
            }

            public Progress? DeleteProgress(Guid progressId)
            {
                return _mock.Object.DeleteProgress(progressId);
            }
        }
        public class ProgressRepositorySpy : IProgressRepository
        {
            public List<string> MethodCalls { get; } = new List<string>();

            public IQueryable<Progress> GetProgresses()
            {
                MethodCalls.Add(nameof(GetProgresses));
                return new List<Progress>().AsQueryable(); 
            }

            public Progress? GetProgressById(Guid progressId)
            {
                MethodCalls.Add(nameof(GetProgressById));
                return new Progress { Id = progressId }; 
            }

            public Progress AddProgress(Progress progress)
            {
                MethodCalls.Add(nameof(AddProgress));
                return progress; 
            }

            public Progress? UpdateProgress(Progress progress)
            {
                MethodCalls.Add(nameof(UpdateProgress));
                return progress; 
            }

            public Progress? DeleteProgress(Guid progressId)
            {
                MethodCalls.Add(nameof(DeleteProgress));
                return null; 
            }
        }
    }
}
