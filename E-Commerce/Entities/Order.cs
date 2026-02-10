using E_Commerce.Contract;
using E_Commerce.Enums;

namespace E_Commerce.Entities
{
    public class Order : ISoftDeleteable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string UserId { get; set; } = null!;
        public required decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public required string ShippingCity { get; set; } = null!;
        public required string ShippingStreet { get; set; } = null!;
        public required string ReceiverPhone { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public Payment? Payment { get; set; }
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

