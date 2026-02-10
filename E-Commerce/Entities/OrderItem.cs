using E_Commerce.Contract;

namespace E_Commerce.Entities
{
    public class OrderItem : ISoftDeleteable
    { 
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public required Guid ProductId { get; set; }
        public required int Quantity { get; set; }
        public required decimal PriceAtPurchase { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
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

