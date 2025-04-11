using Model;

namespace IDAL;

public interface IStageRepository
{
    Task<IEnumerable<Stage>> GetStagesAsync();
    Task<Stage?> GetStageByIdAsync(Guid stageId);
    Task AddStageAsync(Stage stage);
    Task UpdateStageAsync(Stage stage);
    Task DeleteStageAsync(Guid stageId);
}
