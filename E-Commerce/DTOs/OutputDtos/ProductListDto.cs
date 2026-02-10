namespace E_Commerce.DTOs.OutputDtos
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal FinalPrice => SalePrice > 0 ? SalePrice : Price;
        public string CategoryName { get; set; } = null!;

        public string MainImageUrl { get; set; } = null!;

        public double AverageRate { get; set; }
        public int ReviewsCount { get; set; }
        public bool IsInStock => Quantity > 0;
        public int Quantity { get; set; }

    }
}
