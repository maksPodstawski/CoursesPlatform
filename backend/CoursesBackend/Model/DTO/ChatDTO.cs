using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record CreateChatDTO
    {
        [Required]
        public required string Name { get; init; }
    }

    public record CreateChatResponseDTO 
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid CourseId { get; init; }
        public string CourseName { get; init; } = string.Empty;
    }

}
