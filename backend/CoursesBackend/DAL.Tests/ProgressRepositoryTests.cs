using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Tests
{
    public class ProgressRepositoryTests
    {
        private DbContextOptions<CoursesPlatformContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<CoursesPlatformContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetProgressById_WhenNotFound_ReturnsNull()
        {
            var options = CreateNewContextOptions();
            using var context = new CoursesPlatformContext(options);
            var repo = new ProgressRepository(context);

            var result = repo.GetProgressById(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public void GetProgresses_ReturnsAll()
        {
            var options = CreateNewContextOptions();
            var progresses = new List<Progress>
        {
            new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = Guid.NewGuid() },
            new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = Guid.NewGuid() }
        };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Progresses.AddRange(progresses);
                context.SaveChanges();
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ProgressRepository(context);
                var result = repo.GetProgresses().ToList();
                Assert.Equal(2, result.Count);
            }
        }

        [Fact]
        public void AddProgress_SavesToDatabase()
        {
            var options = CreateNewContextOptions();
            var progress = new Progress { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), StageId = Guid.NewGuid() };

            using (var context = new CoursesPlatformContext(options))
            {
                var repo = new ProgressRepository(context);
                repo.AddProgress(progress);
            }

            using (var context = new CoursesPlatformContext(options))
            {
                var saved = context.Progresses.Single();
                Assert.Equal(progress.Id, saved.Id);
            }
        }

        [Fact]
        public void UpdateProgress_ChangesAreSaved()
        {
            var options = CreateNewContextOptions();
            var progressId = Guid.NewGuid();

            var original = new Progress
            {
                Id = progressId,
                UserId = Guid.NewGuid(),
                StageId = Guid.NewGuid(),
                IsCompleted = false
            };

            var updated = new Progress
            {
                Id = progressId,
                UserId = original.UserId,
                StageId = original.StageId,
                IsCompleted = true
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Progresses.Add(original);
                context.SaveChanges();
            }

            var spyContext = new DbContextSpy(options);
            var repo = new ProgressRepository(spyContext);
            var result = repo.UpdateProgress(updated);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public void DeleteProgress_RemovesAndReturnsProgress()
        {
            var options = CreateNewContextOptions();
            var id = Guid.NewGuid();

            var toDelete = new Progress
            {
                Id = id,
                UserId = Guid.NewGuid(),
                StageId = Guid.NewGuid()
            };

            using (var context = new CoursesPlatformContext(options))
            {
                context.Progresses.Add(toDelete);
                context.SaveChanges();
            }

            var spyContext = new DbContextSpy(options);
            var repo = new ProgressRepository(spyContext);
            var result = repo.DeleteProgress(id);

            Assert.Equal(1, spyContext.SaveChangesCallCount);
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);

            using (var context = new CoursesPlatformContext(options))
            {
                Assert.Empty(context.Progresses.ToList());
            }
        }

        private class DbContextSpy : CoursesPlatformContext
        {
            public int SaveChangesCallCount { get; private set; } = 0;

            public DbContextSpy(DbContextOptions<CoursesPlatformContext> options) : base(options) { }

            public override int SaveChanges()
            {
                SaveChangesCallCount++;
                return base.SaveChanges();
            }
        }
    }
}
