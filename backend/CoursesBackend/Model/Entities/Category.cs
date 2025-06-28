using Model.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<Subcategory>? Subcategories { get; set; } = new List<Subcategory>();

        public static Category FromDTO(CategoryNameDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new Category { Name = dto.Name };
        }
        public CategoryDTO ToDTO()
        {
            return new CategoryDTO
            {
                Id = this.Id,
                Name = this.Name
            };
        }
    }
}
