using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputCategoryDto
    {
        [Required(ErrorMessage = "Name Is Required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }
}
