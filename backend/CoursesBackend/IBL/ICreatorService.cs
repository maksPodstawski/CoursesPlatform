using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface ICreatorService
    {
        IQueryable<Creator> GetAllCreatorsAsync();
        Task<Creator?> GetCreatorByIdAsync(Guid creatorId);
        Task AddCreatorAsync(Creator creator);
        Task<bool> DeleteCreatorAsync(Guid creatorId);
        IQueryable<Course> GetCoursesByCreatorAsync(Guid userId);
        bool IsUserCreatorOfCourseAsync(Guid userId, Guid courseId);

        Task<Creator> AddCreatorFromUserAsync(Guid userId, Guid courseId);
    }
}
