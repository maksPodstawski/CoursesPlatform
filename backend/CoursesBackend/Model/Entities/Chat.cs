using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model.DTO;

namespace Model
{
    [Table("Chats")]
    [Index(nameof(CourseId), nameof(ChatAuthorId), IsUnique = true)]
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Course))]
        public Guid CourseId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid ChatAuthorId { get; set; }

        public ICollection<ChatUser>? Users { get; set; } = new List<ChatUser>();
        public ICollection<Message>? Messages { get; set; } = new List<Message>();
        public static Chat ChatFromDTO(CreateChatDTO dto, Guid authorId, Guid courseId)
        {
            return new Chat
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ChatAuthorId = authorId,
                CourseId = courseId
            };
        }
        public static Chat ChatFromDTO(string name, Guid authorId, Guid courseId)
        {
            return new Chat
            {
                Id = Guid.NewGuid(),
                Name = name,
                ChatAuthorId = authorId,
                CourseId = courseId
            };
        }
    }

}
