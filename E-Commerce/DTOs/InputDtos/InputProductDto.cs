namespace E_Commerce.DTOs.InputDtos
{
    public class InputProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal SalePrice { get; set; }
        public int Quantity { get; set; }
        public Guid CategoryId { get; set; }
        public ICollection<InputProductImageDto> Images { get; set; } = new List<InputProductImageDto>();
    }
    public class InputProductImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}

