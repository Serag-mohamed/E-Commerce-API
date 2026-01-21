namespace E_Commerce.DTOs.OutputDtos
{
    public class VendorStatsDto
    {
        public int MyTotalProducts { get; set; }
        public decimal MyTotalSales { get; set; } 
        public int MyPendingOrdersCount { get; set; } 
        public int MyLowStockCount { get; set; }
        public double AverageRating { get; set; } 
    }
}
