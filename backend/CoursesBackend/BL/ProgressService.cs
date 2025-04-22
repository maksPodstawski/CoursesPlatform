using IBL;
using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _progressRepository;

        public ProgressService(IProgressRepository progressRepository)
        {
            _progressRepository = progressRepository;
        }

        public IQueryable<Progress> GetAllProgressesAsync()
        {
            return  _progressRepository.GetProgresses();
        }

        public async Task<Progress?> GetProgressByIdAsync(Guid progressId)
        {
            return await _progressRepository.GetProgressByIdAsync(progressId);
        }

        public IQueryable<Progress> GetProgressByUserIdAsync(Guid userId)
        {
            var progresses = _progressRepository.GetProgresses();
            return progresses.Where(p => p.UserId == userId);
        }

        public IQueryable<Progress> GetProgressByStageIdAsync(Guid stageId)
        {
            var progresses =  _progressRepository.GetProgresses();
            return progresses.Where(p => p.StageId == stageId);
        }

        public async Task AddProgressAsync(Progress progress)
        {
            progress.Id = Guid.NewGuid();
            progress.StartedAt = DateTime.UtcNow;
            progress.LastAccessedAt = DateTime.UtcNow;
            await _progressRepository.AddProgressAsync(progress);
        }

        public async Task UpdateProgressAsync(Progress progress)
        {
            await _progressRepository.UpdateProgressAsync(progress);
        }

        public async Task DeleteProgressAsync(Guid progressId)
        {
            await _progressRepository.DeleteProgressAsync(progressId);
        }

        public async Task MarkStageAsCompletedAsync(Guid userId, Guid stageId)
        {
            var progresses = _progressRepository.GetProgresses();
            var progress = progresses.FirstOrDefault(p => p.UserId == userId && p.StageId == stageId);

            if (progress != null)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                await _progressRepository.UpdateProgressAsync(progress);
            }
        }
    }

 }
