namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputProductDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public decimal FinalPrice => SalePrice > 0 ? SalePrice : Price;
        public int Quantity { get; set; }

        public string CategoryName { get; set; } = null!;
        public ICollection<OutputProductImagesDto> Images { get; set; } = [];
        public ICollection<OutputReviewDto> Reviews { get; set; } = [];
    }
    public class OutputProductImagesDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }

    }
    public class OutputReviewDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = null!;
        public int Rate { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    }
}