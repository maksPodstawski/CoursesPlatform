using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace IDAL
{
    public interface IChatUserRepository
    {
        Task<IEnumerable<ChatUser>> GetAllAsync();
        Task<ChatUser?> GetByIdAsync(Guid id);
        Task AddAsync(ChatUser chatUser);
        Task UpdateAsync(ChatUser chatUser);
        Task DeleteAsync(Guid id);
    }
}
