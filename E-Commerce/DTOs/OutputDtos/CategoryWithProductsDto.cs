namespace E_Commerce.DTOs.OutputDtos
{
    public class CategoryWithProductsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ParentCategoryName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }

        public ICollection<ProductListDto> Products { get; set; } = [];
    }
}
