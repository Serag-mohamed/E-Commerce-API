
namespace E_Commerce.Entities
{
    public class Address
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string HomePositionDescription { get; set; } = string.Empty;

        public ApplicationUser User { get; set; }
    }
}

