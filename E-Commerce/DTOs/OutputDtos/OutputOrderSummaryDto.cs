namespace E_Commerce.DTOs.OutputDtos
{
    public class OutputOrderSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemsCount { get; set; }
    }
}
