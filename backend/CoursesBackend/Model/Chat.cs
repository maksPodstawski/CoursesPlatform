using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Chats")]
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public ICollection<ChatUser>? Users { get; set; } = new List<ChatUser>();
        public ICollection<Message>? Messages { get; set; } = new List<Message>();
    }
}
