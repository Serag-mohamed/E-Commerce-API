namespace E_Commerce.Entities
{
    public class ProductImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; } = false;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

