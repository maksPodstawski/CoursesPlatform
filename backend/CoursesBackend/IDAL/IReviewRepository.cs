using Model;

namespace IDAL;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review?> GetByIdAsync(Guid id);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Guid id);
}