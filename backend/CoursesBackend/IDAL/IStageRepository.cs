using Model;

namespace IDAL;

public interface IStageRepository
{
    IQueryable<Stage> GetStages();
    Task<Stage?> GetStageByIdAsync(Guid stageId);
    Task AddStageAsync(Stage stage);
    Task UpdateStageAsync(Stage stage);
    Task DeleteStageAsync(Guid stageId);
}
