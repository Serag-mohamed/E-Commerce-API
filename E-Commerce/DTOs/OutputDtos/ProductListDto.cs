namespace E_Commerce.DTOs.OutputDtos
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal TotalPrice => Price - DiscountPrice;
        public string CategoryName { get; set; } = string.Empty;

        public string MainImageUrl { get; set; } = string.Empty;

        public double AverageRate { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsInStock => Quantity > 0;
        public int Quantity { get; set; }

    }
}
