using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Enums;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class OrderService(IUnitOfWork unitOfWork)
    {
        public async Task<OperationResult<Guid>> CheckOutAsync(string userId, CheckoutDto checkoutDto)
        {
            var address = await unitOfWork.Repository<Address>().Query()
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == checkoutDto.AddressId);

            if (address == null || address.UserId != userId)
                return new OperationResult<Guid> { IsSucceeded = false, Message = "Invalid address." };

            var cart = await unitOfWork.Repository<Cart>().Query()
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                return new OperationResult<Guid> { IsSucceeded = false, Message = "Cart is empty." };

            decimal calculatedTotal = 0;
            foreach (var item in cart.Items)
            {
                if (item.Product.Quantity < item.Quantity)
                    return new OperationResult<Guid>
                    {
                        IsSucceeded = false,
                        Message = $"Insufficient stock for product {item.Product.Name}."
                    };

                calculatedTotal += item.Product.FinalPrice * item.Quantity;
            }

            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
            var order = new Order
            {
                UserId = userId,
                TotalPrice = calculatedTotal,
                ShippingCity = address.City,
                ShippingStreet = address.Street,
                ReceiverPhone = address.User.PhoneNumber!,
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    PriceAtPurchase = i.Product.FinalPrice
                }).ToList()
            };

            foreach (var item in cart.Items)
                item.Product.Quantity -= item.Quantity;
            
                await unitOfWork.Repository<Order>().AddAsync(order);
                unitOfWork.Repository<Cart>().Delete(cart);

                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OperationResult<Guid> { IsSucceeded = true, Data = order.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult<Guid> { IsSucceeded = false, Message = $"Checkout failed: {ex.Message}" };
            }
        }
        public async Task<List<OrderSummaryDto>> GetOrdersAsync(string userId, bool isAdmin)
        {
            var orders = await unitOfWork.Repository<Order>().Query()
                .AsNoTracking()
                .Where(o => o.UserId == userId || isAdmin)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    OrderDate = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    Status = o.OrderStatus.ToString(),
                    ItemsCount = o.OrderItems.Count
                })
                .ToListAsync();

            return orders;
        }
        public async Task<OperationResult<OrderDto>> GetOrderDetaialsAsync(Guid orderId, string userId, bool isAdmin)
        {
            var order = await unitOfWork.Repository<Order>().Query()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == orderId && isAdmin || o.UserId == userId);

            if (order == null)
                return new OperationResult<OrderDto> { IsSucceeded = false, Message = "Order not found" };

            var dto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                Status = order.OrderStatus.ToString(),
                ShippingCity = order.ShippingCity,
                ShippingStreet = order.ShippingStreet,
                ReceiverPhone = order.ReceiverPhone,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    ProductImageUrl = oi.Product.ProductImages.FirstOrDefault(img => img.IsMain == true)!.ImageUrl,
                    Quantity = oi.Quantity,
                    PriceAtPurchase = oi.PriceAtPurchase
                }).ToList()
            };

            return new OperationResult<OrderDto> { IsSucceeded = true, Data = dto };
        }
        public async Task<OperationResult> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await unitOfWork.Repository<Order>().GetByIdAsync(orderId);

            if (order == null)
                return new OperationResult { IsSucceeded = false, Message = "Order not found." };

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled)
            {
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Cannot change status of an order that is already {order.OrderStatus}."
                };
            }

            order.OrderStatus = newStatus;

            await unitOfWork.SaveChangesAsync();

            return new OperationResult { IsSucceeded = true, Message = $"Order status updated to {newStatus} successfully." };
        }
        public async Task<bool> HasUserPurchasedProductAsync(string userId, Guid productId)
        {
            return await unitOfWork.Repository<Order>().Query()
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId) && o.OrderStatus == OrderStatus.Delivered);
        }
    }
}
