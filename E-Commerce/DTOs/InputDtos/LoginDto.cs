using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
