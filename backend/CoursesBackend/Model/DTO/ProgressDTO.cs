using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class ProgressDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid StageId { get; set; }
        public DateTime LastAccessedAt { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public static ProgressDTO FromEntity(Progress progress)
        {
            if (progress == null) return null;

            return new ProgressDTO
            {
                Id = progress.Id,
                UserId = progress.UserId,
                StageId = progress.StageId,
                LastAccessedAt = progress.LastAccessedAt,
                IsCompleted = progress.IsCompleted,
                StartedAt = progress.StartedAt,
                CompletedAt = progress.CompletedAt
            };
        }

        public Progress ToEntity()
        {
            return new Progress
            {
                Id = this.Id,
                UserId = this.UserId,
                StageId = this.StageId,
                LastAccessedAt = this.LastAccessedAt,
                IsCompleted = this.IsCompleted,
                StartedAt = this.StartedAt,
                CompletedAt = this.CompletedAt
            };
        }
    }
}
