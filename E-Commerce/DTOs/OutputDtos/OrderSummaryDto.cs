namespace E_Commerce.DTOs.OutputDtos
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public int ItemsCount { get; set; }
    }
}
