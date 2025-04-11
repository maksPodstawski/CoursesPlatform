using Model;

namespace IBL
{
    public interface IStageService
    {
        Task<IEnumerable<Stage>> GetAllStagesAsync();
        Task<Stage?> GetStageByIdAsync(Guid id);
        Task<Stage> AddStageAsync(Stage stage);
        Task<Stage?> UpdateStageAsync(Stage stage);
        Task<Stage?> DeleteStageAsync(Guid id);
    }
}
