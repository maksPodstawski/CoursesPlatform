using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Progresses")]
    public class Progress
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public Guid StageId { get; set; }

        [ForeignKey(nameof(StageId))]
        public Stage Stage { get; set; }


        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; } = null;

    }
}
