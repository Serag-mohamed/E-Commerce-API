using E_Commerce.Enums;

namespace E_Commerce.DTOs.OutputDtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();
        public string ShippingCity { get; set; } = null!;
        public string ShippingStreet { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public List<OrderItemDto> OrderItems { get; set; } = [];
    }
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductImageUrl { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}
