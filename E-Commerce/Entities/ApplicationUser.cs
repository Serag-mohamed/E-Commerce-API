using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string DisplayName { get; set; } = null!;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<Address> Addresses { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
        public ICollection<Cart> Carts { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Product> Products { get; set; } = [];
    }
}
