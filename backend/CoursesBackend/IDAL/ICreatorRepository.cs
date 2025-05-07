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
        Creator? GetCreatorByID(Guid creatorID);
        Creator AddCreator(Creator creator);
        Creator? UpdateCreator(Creator creator);
        Creator? DeleteCreator(Guid creatorID);
    }
}
