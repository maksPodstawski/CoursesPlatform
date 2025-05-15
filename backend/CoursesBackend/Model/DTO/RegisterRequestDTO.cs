using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record RegisterRequestDTO
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
