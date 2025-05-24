using Model;
using Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IProgressService
    {
        Task<List<Progress>> GetAllProgressesAsync();
        Task<Progress?> GetProgressByIdAsync(Guid progressId);
        Task<List<Progress>> GetProgressByUserIdAsync(Guid userId);
        Task<List<Progress>> GetProgressByStageIdAsync(Guid stageId);
        Task<Progress> AddProgressAsync(Progress progress);
        Task<Progress?> UpdateProgressAsync(Progress progress);
        Task<Progress?> DeleteProgressAsync(Guid progressId);
        Task MarkStageAsCompletedAsync(Guid userId, Guid stageId);
        Task<List<StageWithProgressDto>> GetStagesWithProgressForCourseAsync(Guid userId, Guid courseId);
    }
}
