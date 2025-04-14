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

        public async Task<IEnumerable<Progress>> GetAllProgressesAsync()
        {
            return await _progressRepository.GetProgressesAsync();
        }

        public async Task<Progress?> GetProgressByIdAsync(Guid progressId)
        {
            return await _progressRepository.GetProgressByIdAsync(progressId);
        }

        public async Task<IEnumerable<Progress>> GetProgressByUserIdAsync(Guid userId)
        {
            var progresses = await _progressRepository.GetProgressesAsync();
            return progresses.Where(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Progress>> GetProgressByStageIdAsync(Guid stageId)
        {
            var progresses = await _progressRepository.GetProgressesAsync();
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
            var progresses = await _progressRepository.GetProgressesAsync();
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
