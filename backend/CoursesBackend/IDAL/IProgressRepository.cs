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
        Progress? GetProgressById(Guid progressId);
        Progress AddProgress(Progress progress);
        Progress? UpdateProgress(Progress progress);
        Progress? DeleteProgress(Guid progressId);
    }
}
