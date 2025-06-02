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
        Task<List<Creator>> GetAllCreatorsAsync();
        Task<Creator?> GetCreatorByIdAsync(Guid creatorId);
        Task<Creator> AddCreatorAsync(Creator creator);
        Task<Creator?> DeleteCreatorAsync(Guid creatorId);
        Task<List<Course>> GetCoursesByCreatorAsync(Guid userId);
        Task<bool> IsUserCreatorOfCourseAsync(Guid userId, Guid courseId);
        Task<List<Creator>> GetCreatorsByCourseAsync(Guid courseId);
        Task<Creator> AddCreatorFromUserAsync(Guid userId, Guid courseId);
    }
}
