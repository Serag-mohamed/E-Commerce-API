namespace E_Commerce.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CartId { get; set; }
        public required Guid ProductId { get; set; }
        public required int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
