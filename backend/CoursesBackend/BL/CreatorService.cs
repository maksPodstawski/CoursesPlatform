using IBL;
using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CreatorService : ICreatorService
    {
        private readonly ICreatorRepository _creatorRepository;
        private readonly ICourseRepository _courseRepository;

        public CreatorService(ICreatorRepository creatorRepository, ICourseRepository courseRepository)
        {
            _creatorRepository = creatorRepository;
            _courseRepository = courseRepository;
        }

        public IQueryable<Creator> GetAllCreatorsAsync()
        {
            return _creatorRepository.GetCreators();
        }

        public async Task<Creator?> GetCreatorByIdAsync(Guid creatorId)
        {
            return await _creatorRepository.GetCreatorByIDAsync(creatorId);
        }

        public async Task AddCreatorAsync(Creator creator)
        {
            await _creatorRepository.AddCreatorAsync(creator);
        }

        public async Task<bool> DeleteCreatorAsync(Guid creatorId)
        {
            var existing = await _creatorRepository.GetCreatorByIDAsync(creatorId);
            if (existing == null) return false;

            await _creatorRepository.DeleteCreatorAsync(creatorId);
            return true;
        }

        public IQueryable<Course> GetCoursesByCreatorAsync(Guid userId)
        {
            var creators =  _creatorRepository.GetCreators();
            var courseIds = creators
                .Where(c => c.UserId == userId)
                .Select(c => c.CourseId)
                .ToList();

            var allCourses =  _courseRepository.GetCourses();
            return allCourses.Where(c => courseIds.Contains(c.Id));
        }

        public bool IsUserCreatorOfCourseAsync(Guid userId, Guid courseId)
        {
            var creators =  _creatorRepository.GetCreators();
            return creators.Any(c => c.UserId == userId && c.CourseId == courseId);
        }
        public async Task<Creator> AddCreatorFromUserAsync(Guid userId, Guid courseId)
        {
            var newCreator = new Creator
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CourseId = courseId
            };

            await _creatorRepository.AddCreatorAsync(newCreator);
            return newCreator;
        }
    }
}
