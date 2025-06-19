using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record RegisterRequestDTO
    {
        [Required(ErrorMessage = "The first name field is required.")]
        [StringLength(50)]
        public required string FirstName { get; init; }
        [Required(ErrorMessage = "The last name field is required.")]
        [StringLength(50)]
        public required string LastName { get; init; }
        [Required(ErrorMessage = "The email address field is required.")]
        [EmailAddress]
        public required string Email { get; init; }
        [Required(ErrorMessage = "The password field is required.")]
        [MinLength(8, ErrorMessage = "The password must have at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""{}|<>]).*$", 
            ErrorMessage = "The password must contain at least one uppercase letter, one lowercase letter, and one special character.")]
        public required string Password { get; init; }

        [Required(ErrorMessage = "The password confirmation field is required.")]
        [Compare(nameof(Password), ErrorMessage = "Confirmation password and password must be the same.")]
        public required string ConfirmPassword { get; init; }

        [Required(ErrorMessage = "The recaptcha token field is required.")]
        public required string RecaptchaToken { get; init;}
    }
}
