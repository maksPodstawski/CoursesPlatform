using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface ICreatorRepository
    {
        IQueryable<Creator> GetCreators();
        Task<Creator?> GetCreatorByIDAsync(Guid creatorID);
        Task AddCreatorAsync(Creator creator);
        Task UpdateCreatorAsync(Creator creator);
        Task DeleteCreatorAsync(Guid creatorID);
    }
}
