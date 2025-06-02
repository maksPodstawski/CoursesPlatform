using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _progressRepository;
        private readonly IStageRepository _stageRepository;

        public ProgressService(IProgressRepository progressRepository, IStageRepository stageRepository)
        {
            _progressRepository = progressRepository;
            _stageRepository = stageRepository;
        }

        public async Task<List<Progress>> GetAllProgressesAsync()
        {
            return await _progressRepository.GetProgresses().ToListAsync();
        }

        public async Task<Progress?> GetProgressByIdAsync(Guid progressId)
        {
            return await Task.FromResult(_progressRepository.GetProgressById(progressId));
        }

        public async Task<List<Progress>> GetProgressByUserIdAsync(Guid userId)
        {
            return await _progressRepository.GetProgresses()
                 .Where(p => p.UserId == userId)
                 .ToListAsync();
        }

        public async Task<List<Progress>> GetProgressByStageIdAsync(Guid stageId)
        {
            return await _progressRepository.GetProgresses()
               .Where(p => p.StageId == stageId)
               .ToListAsync();
        }

        public async Task<Progress> AddProgressAsync(Progress progress)
        {
            progress.Id = Guid.NewGuid();
            progress.StartedAt = DateTime.UtcNow;
            progress.LastAccessedAt = DateTime.UtcNow;
            return await Task.FromResult(_progressRepository.AddProgress(progress));
        }

        public async Task<Progress?> UpdateProgressAsync(Progress progress)
        {
            var existing = await Task.FromResult(_progressRepository.GetProgressById(progress.Id));
            if (existing == null) return null;

            return await Task.FromResult(_progressRepository.UpdateProgress(progress));
        }

        public async Task<Progress?> DeleteProgressAsync(Guid progressId)
        {
            var progress = await Task.FromResult(_progressRepository.GetProgressById(progressId));
            if (progress == null) return null;

            return await Task.FromResult(_progressRepository.DeleteProgress(progressId));
        }

        public async Task MarkStageAsCompletedAsync(Guid userId, Guid stageId)
        {
            var progress = await _progressRepository.GetProgresses()
                 .FirstOrDefaultAsync(p => p.UserId == userId && p.StageId == stageId);

            if (progress != null && !progress.IsCompleted)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                await Task.FromResult(_progressRepository.UpdateProgress(progress));
            }
        }

        public async Task<List<StageWithProgressDto>> GetStagesWithProgressForCourseAsync(Guid userId, Guid courseId)
        {
            var stages = await _stageRepository.GetStages()
                .Where(s => s.CourseId == courseId)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync();

            var progress = await _progressRepository.GetProgresses()
                .Where(p => p.UserId == userId && stages.Select(s => s.Id).Contains(p.StageId))
                .ToListAsync();

            return stages.Select(stage =>
            {
                var stageProgress = progress.FirstOrDefault(p => p.StageId == stage.Id);
                return new StageWithProgressDto
                {
                    Id = stage.Id,
                    Name = stage.Name,
                    Description = stage.Description,
                    Duration = stage.Duration,
                    Order = stages.IndexOf(stage),
                    IsCompleted = stageProgress?.IsCompleted ?? false,
                    StartedAt = stageProgress?.StartedAt,
                    CompletedAt = stageProgress?.CompletedAt,
                    LastAccessedAt = stageProgress?.LastAccessedAt
                };
            }).ToList();
        }
    }
}
