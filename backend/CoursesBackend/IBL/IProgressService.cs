using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IProgressService
    {
        IQueryable<Progress> GetAllProgressesAsync();
        Task<Progress?> GetProgressByIdAsync(Guid progressId);
        IQueryable<Progress> GetProgressByUserIdAsync(Guid userId);
        IQueryable<Progress> GetProgressByStageIdAsync(Guid stageId);
        Task AddProgressAsync(Progress progress);
        Task UpdateProgressAsync(Progress progress);
        Task DeleteProgressAsync(Guid progressId);
        Task MarkStageAsCompletedAsync(Guid userId, Guid stageId);
    }
}
