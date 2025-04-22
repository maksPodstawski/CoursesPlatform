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
            return _context.Creators.AsQueryable();
        }

        public async Task<Creator?> GetCreatorByIDAsync(Guid creatorID)
        {
            return await _context.Creators.FindAsync(creatorID);
        }

        public async Task AddCreatorAsync(Creator creator)
        {
            await _context.Creators.AddAsync(creator);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCreatorAsync(Creator creator)
        {
            _context.Entry(creator).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCreatorAsync(Guid creatorID)
        {
            var creator = await GetCreatorByIDAsync(creatorID);
            if (creator != null)
            {
                _context.Creators.Remove(creator);
                await _context.SaveChangesAsync();
            }
        }
    }
}
