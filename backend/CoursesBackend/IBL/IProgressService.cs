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
        Task<IEnumerable<Progress>> GetAllProgressesAsync();
        Task<Progress?> GetProgressByIdAsync(Guid progressId);
        Task<IEnumerable<Progress>> GetProgressByUserIdAsync(Guid userId);
        Task<IEnumerable<Progress>> GetProgressByStageIdAsync(Guid stageId);
        Task AddProgressAsync(Progress progress);
        Task UpdateProgressAsync(Progress progress);
        Task DeleteProgressAsync(Guid progressId);
        Task MarkStageAsCompletedAsync(Guid userId, Guid stageId);
    }
}
