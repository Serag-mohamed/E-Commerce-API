namespace E_Commerce.DTOs.OutputDtos
{
    public class CategoryWithProductsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ParentCategoryName { get; set; }
        public int TotalProducts { get; set; }

        public ICollection<ProductListDto> Products { get; set; } = new List<ProductListDto>();
    }
}
