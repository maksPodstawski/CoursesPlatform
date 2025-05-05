using Model;

namespace IDAL
{
    public interface IStageRepository
    {
        IQueryable<Stage> GetStages();
        Stage? GetStageById(Guid stageId);
        Stage AddStage(Stage stage);
        Stage? UpdateStage(Stage stage);
        Stage? DeleteStage(Guid stageId);
    }
}


