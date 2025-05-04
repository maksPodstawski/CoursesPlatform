using Model;

namespace IBL
{
    public interface IStageService
    {
        Task<List<Stage>> GetAllStagesAsync();
        Task<Stage?> GetStageByIdAsync(Guid id);
        Task<List<Stage>> GetStagesByNameAsync(string name);
        Task<List<Stage>> GetStagesByCourseIdAsync(Guid courseId);
        Task<Stage> AddStageAsync(Stage stage);
        Task<Stage?> UpdateStageAsync(Stage stage);
        Task<Stage?> DeleteStageAsync(Guid id);
    }
}
