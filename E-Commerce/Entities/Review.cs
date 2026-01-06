namespace E_Commerce.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}

