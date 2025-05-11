using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class CreatorRepository : ICreatorRepository
    {
        private readonly CoursesPlatformContext _context;

        public CreatorRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public IQueryable<Creator> GetCreators()
        {
            return _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses);
        }

        public Creator? GetCreatorByID(Guid creatorID)
        {
            return _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == creatorID);
        }

        public Creator AddCreator(Creator creator)
        {
            _context.Creators.Add(creator);
            _context.SaveChanges();
            return _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == creator.Id)!;
        }

        public Creator? UpdateCreator(Creator creator)
        {
            var existing = _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == creator.Id);
            if (existing == null)
                return null;

            existing.UserId = creator.UserId;
            _context.SaveChanges();
            
            return _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == creator.Id);
        }

        public Creator? DeleteCreator(Guid creatorID)
        {
            var creator = _context.Creators
                .Include(c => c.User)
                .Include(c => c.Courses)
                .FirstOrDefault(c => c.Id == creatorID);
            if (creator == null)
                return null;

            _context.Creators.Remove(creator);
            _context.SaveChanges();
            return creator;
        }
    }
}
