using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync();
        Task<Message?> GetByIdAsync(Guid id);
        Task AddAsync(Message message);
        Task UpdateAsync(Message message);
        Task DeleteAsync(Guid id);
    }
}
