using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class InputReviewDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;
    }
}
