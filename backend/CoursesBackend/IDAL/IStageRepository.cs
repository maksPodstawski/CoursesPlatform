using Model;

namespace IDAL;

public interface IStageRepository
{
    Task<IEnumerable<Stage>> GetAllAsync();
    Task<Stage?> GetByIdAsync(Guid id);
    Task AddAsync(Stage stage);
    Task UpdateAsync(Stage stage);
    Task DeleteAsync(Guid id);
}