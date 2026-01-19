namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputCartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice => Items.Sum(i => i.SubTotal);
    }
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }
}
