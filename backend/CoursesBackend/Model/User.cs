using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        public ICollection<ChatUser>? ChatUsers { get; set; } = new List<ChatUser>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public ICollection<Progress>? Progresses { get; set; } = new List<Progress>();
    }
}
