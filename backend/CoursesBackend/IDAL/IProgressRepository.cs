using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IProgressRepository
    {
        IQueryable<Progress> GetProgresses();
        Task<Progress?> GetProgressByIdAsync(Guid progressId);
        Task AddProgressAsync(Progress progress);
        Task UpdateProgressAsync(Progress progress);
        Task DeleteProgressAsync(Guid progressId);
    }
}
