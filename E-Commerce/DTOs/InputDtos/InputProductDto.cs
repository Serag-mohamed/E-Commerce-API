using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputProductDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public ICollection<InputProductImageDto> Images { get; set; } = [];
    }
    public class InputProductImageDto
    {
        [Required]
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
    }
}

