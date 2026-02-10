using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Enums;
using E_Commerce.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class StatisticsService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        public async Task<AdminStatsDto> GetAdminStatsAsync()
        {
            var productsQuery = unitOfWork.Repository<Product>().Query();
            var ordersQuery = unitOfWork.Repository<Order>().Query();

            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var vendors = await userManager.GetUsersInRoleAsync("Vendor");
            return new AdminStatsDto
            {
                TotalUsers = customers.Count,
                TotalVendors = vendors.Count,
                TotalProducts = await productsQuery.CountAsync(),
                TotalOrders = await ordersQuery.CountAsync(),

                TotalRevenue = await ordersQuery
                    .Where(o => o.OrderStatus == OrderStatus.Delivered)
                    .SumAsync(o => o.TotalPrice),

                LowStockProductsCount = await productsQuery.CountAsync(p => p.Quantity > 0 && p.Quantity < 10),
                OutOfStockCount = await productsQuery.CountAsync(p => p.Quantity == 0),
                PendingOrdersCount = await ordersQuery.CountAsync(o => o.OrderStatus == OrderStatus.Pending)
            };
        }
        public async Task<VendorStatsDto> GetVendorStatsAsync(string vendorId)
        {
            var productsQuery = unitOfWork.Repository<Product>().Query().Where(p => p.VendorId == vendorId);

            var totalSales = await unitOfWork.Repository<OrderItem>().Query()
                .Where(oi => oi.Product.VendorId == vendorId && oi.Order.OrderStatus == OrderStatus.Delivered)
                .SumAsync(oi => oi.PriceAtPurchase * oi.Quantity);

            var avgRating = await unitOfWork.Repository<Review>().Query()
                .Where(r => r.Product.VendorId == vendorId)
                .Select(r => (double?)r.Rate) 
                .AverageAsync() ?? 0;

            return new VendorStatsDto
            {
                MyTotalProducts = await productsQuery.CountAsync(),
                MyTotalSales = totalSales,
                MyLowStockCount = await productsQuery.CountAsync(p => p.Quantity > 0 && p.Quantity < 10),
                MyPendingOrdersCount = await unitOfWork.Repository<OrderItem>().Query()
                    .Where(oi => oi.Product.VendorId == vendorId && oi.Order.OrderStatus == OrderStatus.Pending)
                    .CountAsync(),
                AverageRating = Math.Round(avgRating, 1) 
            };
        }
    }
}
