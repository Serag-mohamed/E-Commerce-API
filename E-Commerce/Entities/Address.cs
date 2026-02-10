
namespace E_Commerce.Entities
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserId { get; set; } = null!;
        public required string City { get; set; } = null!;
        public required string Street { get; set; } = null!;
        public string? HomePositionDescription { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}

