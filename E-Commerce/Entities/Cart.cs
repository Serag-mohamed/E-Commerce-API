namespace E_Commerce.Entities
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public required string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public ICollection<CartItem> Items { get; set; } = [];
    }
}
