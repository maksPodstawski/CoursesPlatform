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
        Task<IEnumerable<Progress>> GetAllAsync();
        Task<Progress?> GetByIdAsync(Guid id);
        Task AddAsync(Progress progress);
        Task UpdateAsync(Progress progress);
        Task DeleteAsync(Guid id);
    }
}
