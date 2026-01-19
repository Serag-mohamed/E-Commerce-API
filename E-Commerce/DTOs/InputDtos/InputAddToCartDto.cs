using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputAddToCartDto
    {
        [Required(ErrorMessage = "*")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
