using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DAL
{
    public class StageRepository : IStageRepository
    {
        private readonly CoursesPlatformContext _context;

        public StageRepository(CoursesPlatformContext context)
        {
            _context = context;
        }

        public IQueryable<Stage> GetStages()
        {
            return _context.Stages;
        }
        public Stage? GetStageById(Guid stageId)
        {
            return _context.Stages.FirstOrDefault(s => s.Id == stageId);
        }
        public Stage AddStage(Stage stage)
        {
            _context.Stages.Add(stage);
            _context.SaveChanges();
            return stage;
        }
        public Stage? UpdateStage(Stage stage)
        {
            var existing = _context.Stages.AsNoTracking().FirstOrDefault(s => s.Id == stage.Id);
            if (existing == null)
                return null;

            existing.Name = stage.Name;
            existing.Description = stage.Description;
            existing.Duration = stage.Duration;
            existing.VideoPath = stage.VideoPath;
            existing.CourseId = stage.CourseId;

            _context.SaveChanges();
            return existing;
        }
        public Stage? DeleteStage(Guid stageId)
        {
            var stage = _context.Stages.FirstOrDefault(s => s.Id == stageId);
            if (stage == null)
                return null;

            _context.Stages.Remove(stage);
            _context.SaveChanges();
            return stage;
        }
    }
}


