using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;

namespace Model.DTO
{
    public record SubcategoryDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid CategoryId { get; set; }
    }

    public record SubcategoryNameDto
    {
        public required string Name { get; set; }
        public Guid CategoryId { get; set; }
    }
}


