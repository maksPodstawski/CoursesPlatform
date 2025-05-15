using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace BL.Services
{
    public class StageService : IStageService
    {
        private readonly IStageRepository _stageRepository;
        public StageService(IStageRepository stageRepository)
        {
            _stageRepository = stageRepository;
        }


        public async Task<List<Stage>> GetAllStagesAsync()
        {
            return await _stageRepository.GetStages().ToListAsync();
        }
        public async Task<Stage?> GetStageByIdAsync(Guid id)
        {
            return await Task.FromResult(_stageRepository.GetStageById(id));
        }
        public async Task<List<Stage>> GetStagesByNameAsync(string name)
        {
            return await _stageRepository.GetStages()
                .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        public async Task<List<Stage>> GetStagesByCourseIdAsync(Guid courseId)
        {
            return await _stageRepository.GetStages()
                .Where(s => s.CourseId == courseId)
                .ToListAsync();
        }



        public async Task<Stage> AddStageAsync(Stage stage)
        {
            return await Task.FromResult(_stageRepository.AddStage(stage));
        }
        public async Task<Stage?> UpdateStageAsync(Stage stage)
        {
            return await Task.FromResult(_stageRepository.UpdateStage(stage));
        }
        public async Task<Stage?> DeleteStageAsync(Guid id)
        {
            return await Task.FromResult(_stageRepository.DeleteStage(id));
        }
    }
}
