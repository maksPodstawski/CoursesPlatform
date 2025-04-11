using Model;

namespace IBL
{
    public interface IStageService
    {
        Task<IEnumerable<Stage>> GetAllStagesAsync();
        Task<Stage?> GetStageByIdAsync(Guid id);
        Task<IEnumerable<Stage>> GetStagesByNameAsync(string name); 
        Task<IEnumerable<Stage>> GetStagesByCourseIdAsync(Guid courseId); 


        Task<Stage> AddStageAsync(Stage stage);
        Task<Stage?> UpdateStageAsync(Stage stage);
        Task<Stage?> DeleteStageAsync(Guid id);
    }
}
