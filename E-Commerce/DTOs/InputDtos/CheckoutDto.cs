using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class CheckoutDto
    {
        [Required]
        public Guid AddressId { get; set; }
    }
}
