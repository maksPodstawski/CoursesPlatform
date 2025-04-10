using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChatsUsers")]
    public class ChatUser
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }


        public Guid ChatId { get; set; }
        [ForeignKey(nameof(ChatId))]
        public Chat Chat { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
