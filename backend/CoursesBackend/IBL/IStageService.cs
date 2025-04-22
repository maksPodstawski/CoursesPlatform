using Model;

namespace IBL
{
    public interface IStageService
    {
        IQueryable<Stage> GetAllStagesAsync();
        Task<Stage?> GetStageByIdAsync(Guid id);
        IQueryable<Stage> GetStagesByNameAsync(string name); 
        IQueryable<Stage> GetStagesByCourseIdAsync(Guid courseId); 


        Task<Stage> AddStageAsync(Stage stage);
        Task<Stage?> UpdateStageAsync(Stage stage);
        Task<Stage?> DeleteStageAsync(Guid id);
    }
}
