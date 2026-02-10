using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs.OutputDtos
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalVendors { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProductsCount { get; set; }
        public int OutOfStockCount { get; set; }  
        public int PendingOrdersCount { get; set; }  
        
    }
}
