using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
    }
}
