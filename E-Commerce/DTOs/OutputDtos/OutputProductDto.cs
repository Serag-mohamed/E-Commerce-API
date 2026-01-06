namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputProductDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal TotalPrice => Price - DiscountPrice;
        public int Quantity { get; set; }

        public string CategoryName { get; set; } = string.Empty;
        public ICollection<OutputProductImagesDto> Images { get; set; } = new List<OutputProductImagesDto>();
        public ICollection<OutputReviewDto> Reviews { get; set; } = new List<OutputReviewDto>();
    }
    public class OutputProductImagesDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }

    }
    public class OutputReviewDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    }
}