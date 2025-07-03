using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Model.DTO;

namespace Model
{
    [Table("Users")]
    public class User : IdentityUser<Guid>
    {
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiresAtUtc { get;set; }

        public ICollection<ChatUser>? ChatUsers { get; set; } = new List<ChatUser>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
        public ICollection<Progress>? Progresses { get; set; } = new List<Progress>();

        [MaxLength(20)]
        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "Numer telefonu może zawierać tylko cyfry (maks. 20 znaków).")]
        public override string? PhoneNumber { get; set; }

        public byte[]? ProfilePicture { get; set; }

        public static User Create(string email, string firstName, string lastName)
        {
            return new User
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
            };
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
        public static User FromDTO(UserDTO dto)
        {
            return new User
            {
                Id = dto.Id,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = null,
                PhoneNumber = null,
                ProfilePicture = null
            };
        }
    }
}
