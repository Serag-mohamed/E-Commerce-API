namespace E_Commerce.DTOs.OutputDtos
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(i => i.SubTotal);
    }
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductImageUrl { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => UnitPrice * Quantity;
    }
}
