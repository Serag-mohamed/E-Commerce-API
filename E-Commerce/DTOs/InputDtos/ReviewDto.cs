using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class ReviewDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }

        [Required]
        public string Comment { get; set; } = null!;
    }
}
