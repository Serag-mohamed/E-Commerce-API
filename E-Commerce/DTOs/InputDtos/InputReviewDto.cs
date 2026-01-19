using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputReviewDto
    {
        [Required(ErrorMessage = "*")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(1, 5, ErrorMessage = "Rate must be between 1 and 5")]
        public int Rate { get; set; }

        [Required(ErrorMessage = "*")]
        public string Comment { get; set; } = string.Empty;
    }
}
