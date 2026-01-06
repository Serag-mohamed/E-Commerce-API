namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputCategoryWithProductsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ParentCategoryName { get; set; }

        public ICollection<OutputProductListDto> Product { get; set; } = new List<OutputProductListDto>();
    }
}
