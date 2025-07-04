using Model.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Messages")]
    public class Message
    {
        public Guid Id { get; set; }
        [Required]
        public Guid ChatId { get; set; }

        [ForeignKey(nameof(ChatId))]
        public Chat Chat { get; set; }
        [Required]
        public Guid AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public User Author { get; set; }
        [MaxLength(500)]
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EditedAt { get; set; } = null;
        public bool IsDeleted { get; set; } = false;
        public static Message MessageFromDTO(MessageDTO dto)
        {
            return new Message
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                ChatId = dto.ChatId,
                AuthorId = dto.AuthorId,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt != default ? dto.CreatedAt : DateTime.UtcNow
            };
        }
    }
}
