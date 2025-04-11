using IBL;
using IDAL;
using Model;

namespace BL
{
    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        public StageService(IStageRepository stageRepository)
        {
            _stageRepository = stageRepository;
        }

        public Task<IEnumerable<Stage>> GetAllStagesAsync()
        {
            return _stageRepository.GetStagesAsync();
        }
        public Task<Stage?> GetStageByIdAsync(Guid id)
        {
            return _stageRepository.GetStageByIdAsync(id);
        }
        public async Task<Stage> AddStageAsync(Stage stage)
        {
            await _stageRepository.AddStageAsync(stage);
            return stage;
        }
        public async Task<Stage?> UpdateStageAsync(Stage stage)
        {
            var existing = await _stageRepository.GetStageByIdAsync(stage.Id);
            if (existing == null)
                return null;

            await _stageRepository.UpdateStageAsync(stage);
            return stage;
        }
        public async Task<Stage?> DeleteStageAsync(Guid id)
        {
            var stage = await _stageRepository.GetStageByIdAsync(id);
            if (stage == null)
                return null;

            await _stageRepository.DeleteStageAsync(id);
            return stage;
        }
    }
}
