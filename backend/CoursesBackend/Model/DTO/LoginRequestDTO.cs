using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record LoginRequestDTO
    {
        [Required(ErrorMessage = "The email address field is required.")]
        [EmailAddress]
        public required string Email { get; init; }
        [Required(ErrorMessage = "The password field is required.")]
        public required string Password { get; init; }
    }
}
