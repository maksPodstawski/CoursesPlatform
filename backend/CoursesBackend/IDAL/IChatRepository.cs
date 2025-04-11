using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<Chat?> GetByIdAsync(Guid id);
        Task AddAsync(Chat chat);
        Task UpdateAsync(Chat chat);
        Task DeleteAsync(Guid id);
    }
}
