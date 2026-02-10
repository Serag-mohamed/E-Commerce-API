using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }
    }
}
