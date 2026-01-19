using E_Commerce.Contract;
using E_Commerce.Enums;

namespace E_Commerce.Entities
{
    public class Payment : ISoftDeleteable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.Pending;
        public Guid TransactionId { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public Order Order { get; set; }
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

