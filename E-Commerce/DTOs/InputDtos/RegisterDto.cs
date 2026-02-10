using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.InputDtos
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string UserName { get; set; } =  null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
