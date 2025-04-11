using Model;

namespace IDAL;

public interface IStageRepository
{
    Task<IEnumerable<Stage>> GetStagesAsync();
    Task<Stage?> GetStageByIdAsync(Guid stageId);
    Task<IEnumerable<Stage>> GetStagesByNameAsync(string name); 
    Task<IEnumerable<Stage>> GetStagesByCourseIdAsync(Guid courseId); 


    Task AddStageAsync(Stage stage);
    Task UpdateStageAsync(Stage stage);
    Task DeleteStageAsync(Guid stageId);
}
