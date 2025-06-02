using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public record CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public record CategoryNameDto
    {
        public string Name { get; set; }
    }
}

