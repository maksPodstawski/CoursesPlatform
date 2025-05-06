using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Creator>> GetAllCreatorsAsync()
        {
            return await _creatorRepository.GetCreators().ToListAsync();
        }

        public async Task<Creator?> GetCreatorByIdAsync(Guid creatorId)
        {
            return await Task.FromResult(_creatorRepository.GetCreatorByID(creatorId));
        }

        public async Task<Creator> AddCreatorAsync(Creator creator)
        {
            return await Task.FromResult(_creatorRepository.AddCreator(creator));
        }

        public async Task<Creator?> DeleteCreatorAsync(Guid creatorId)
        {
            var existing = await Task.FromResult(_creatorRepository.GetCreatorByID(creatorId));
            if (existing == null) return null;

            return await Task.FromResult(_creatorRepository.DeleteCreator(creatorId));
        }

        public async Task<List<Course>> GetCoursesByCreatorAsync(Guid userId)
        {
            return await _creatorRepository.GetCreators()
            .Where(c => c.UserId == userId)
            .SelectMany(c => c.Courses)
            .Distinct() 
            .ToListAsync();
        }

        public async Task<bool> IsUserCreatorOfCourseAsync(Guid userId, Guid courseId)
        {
            return await _creatorRepository.GetCreators()
            .Where(c => c.UserId == userId)
            .AnyAsync(c => c.Courses.Any(course => course.Id == courseId));
        }
        public async Task<Creator> AddCreatorFromUserAsync(Guid userId, Guid courseId)
        {
            var course = await Task.FromResult(_courseRepository.GetCourseById(courseId));
            if (course == null)
            {
                throw new ArgumentException("Course not found");
            }

            var creator = new Creator
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Courses = new List<Course> { course }
            };

            return await Task.FromResult(_creatorRepository.AddCreator(creator));
                    }
    }
}
