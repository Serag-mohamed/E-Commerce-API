namespace E_Commerce.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserId { get; set; } = null!;
        public required Guid ProductId { get; set; }
        public required int Rate { get; set; }
        public required string Comment { get; set; } = null!;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}

