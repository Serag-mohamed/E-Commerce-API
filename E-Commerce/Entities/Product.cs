using E_Commerce.Contract;

namespace E_Commerce.Entities
{
    public class Product : ISoftDeleteable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; } = 0;
        public decimal FinalPrice => SalePrice > 0 ? SalePrice : Price;
        public int Quantity { get; set; }
        public int TotalSalesCount { get; set; } = 0;
        public Guid CategoryId { get; set; }
        public string VendorId { get; set; } = null!;

        public Category Category { get; set; } = null!;
        public ApplicationUser Vendor { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = [];
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public ICollection<CartItem> CartItems { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        public void UndoDelete()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}


