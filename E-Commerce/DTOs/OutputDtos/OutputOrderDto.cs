using E_Commerce.Enums;

namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputOrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingStreet { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public List<OutputOrderItemDto> OrderItems { get; set; } = new List<OutputOrderItemDto>();
    }
    public class OutputOrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
    }
}
