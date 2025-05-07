using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class StageRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetStageById_WhenNotFound_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            using var context = new CoursesPlatformContext(options);
            var repo = new StageRepository(context);

            var result = repo.GetStageById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public void GetStages_ReturnsAllStages()
        {
            var options = CreateNewContextOptions();
            var stages = new List<Stage>
        {
            new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "Stage 1",
                Description = "Desc 1",
                Duration = 10.5
            },
            new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "Stage 2",
                Description = "Desc 2",
                Duration = 15.0
            }
        };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Stages.AddRange(stages);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new StageRepository(context);
                var result = repo.GetStages().ToList();
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void AddStage_SavesToDatabase()
        {
            var options = CreateNewContextOptions();
            var stage = new Stage
            {
                Id = Guid.NewGuid(),
                CourseId = Guid.NewGuid(),
                Name = "New Stage",
                Description = "Learn this",
                Duration = 20.0
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new StageRepository(context);
                repo.AddStage(stage);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var saved = context.Stages.Single();
                Assert.Equal(stage.Name, saved.Name);
            }
        }

        [Fact]
        public void UpdateStage_ChangesAreSaved()
        {
            var options = CreateNewContextOptions();
            var stageId = Guid.NewGuid();

            var original = new Stage
            {
                Id = stageId,
                CourseId = Guid.NewGuid(),
                Name = "Original Name",
                Description = "Original Description",
                Duration = 5.0
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Stages.Add(original);
                context.SaveChanges();
            }

            var updated = new Stage
            {
                Id = stageId,
                CourseId = original.CourseId,
                Name = "Updated Name",
                Description = "Updated Description",
                Duration = 10.0,
                VideoPath = "newvideo.mp4"
            };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new StageRepository(context);
                var result = repo.UpdateStage(updated);

                Assert.NotNull(result);
                Assert.Equal("Updated Name", result?.Name);
                Assert.Equal("newvideo.mp4", result?.VideoPath);
            }
        }

        [Fact]
        public void DeleteStage_RemovesAndReturnsStage()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            var stage = new Stage
            {
                Id = id,
                CourseId = Guid.NewGuid(),
                Name = "Delete Me",
                Description = "To delete",
                Duration = 8.0
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Stages.Add(stage);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new StageRepository(context);
                var result = repo.DeleteStage(id);

                Assert.NotNull(result);
                Assert.Equal("Delete Me", result?.Name);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Stages.ToList());
            }
        }
    }
}
