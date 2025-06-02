using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record MessageDTO
    {
        public Guid Id { get; init; }
        public Guid ChatId { get; init; }
        public Guid AuthorId { get; init; }
        public string AuthorName { get; init; } = "";
        public string Content { get; init; } = "";
        public DateTime CreatedAt { get; init; }
    }
}
