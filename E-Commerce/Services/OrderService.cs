using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Enums;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult<Guid>> CheckOutAsync(string userId, CheckoutDto checkoutDto)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var address = await _unitOfWork.Repository<Address>().Query()
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == checkoutDto.AddressId);
                if (address == null || address.UserId != userId)
                    return new OperationResult<Guid> { Succeeded = false, Message = "Invalid address." };

                var cart = await _unitOfWork.Repository<Cart>().Query()
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                    return new OperationResult<Guid> { Succeeded = false, Message = "Cart is empty." };

                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = cart.Items.Sum(i => i.Product.Price * i.Quantity),
                    ShippingCity = address.City,
                    ShippingStreet = address.Street,
                    ReceiverPhone = address.User.PhoneNumber!,
                    OrderItems = cart.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        PriceAtPurchase = i.Product.Price
                    }).ToList()
                };

                foreach (var item in cart.Items)
                {
                    if (item.Product.Quantity < item.Quantity)
                        return new OperationResult<Guid>
                        {
                            Succeeded = false,
                            Message = $"Insufficient stock for product {item.Product.Name}."
                        };

                    item.Product.Quantity -= item.Quantity;
                }

                await _unitOfWork.Repository<Order>().AddAsync(order);
                _unitOfWork.Repository<Cart>().Delete(cart);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OperationResult<Guid> { Succeeded = true, Data = order.Id };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OperationResult<Guid> { Succeeded = false, Message = $"Checkout failed: {ex.Message}" };
            }
        }
        public async Task<List<OrderSummaryDto>> GetOrdersAsync(string userId, bool isAdmin)
        {
            var orders = await _unitOfWork.Repository<Order>().Query()
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
        public async Task<OperationResult<OutputOrderDto>> GetOrderDetaialsAsync(Guid orderId, string userId, bool isAdmin)
        {
            var order = await _unitOfWork.Repository<Order>().Query()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == orderId && isAdmin || o.UserId == userId);

            if (order == null)
                return new OperationResult<OutputOrderDto> { Succeeded = false, Message = "Order not found" };

            var dto = new OutputOrderDto
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

            return new OperationResult<OutputOrderDto> { Succeeded = true, Data = dto };
        }
        public async Task<OperationResult> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);

            if (order == null)
                return new OperationResult { Succeeded = false, Message = "Order not found." };

            if (order.OrderStatus == OrderStatus.Delivered || order.OrderStatus == OrderStatus.Cancelled)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Cannot change status of an order that is already {order.OrderStatus}."
                };
            }

            order.OrderStatus = newStatus;

            await _unitOfWork.SaveChangesAsync();

            return new OperationResult { Succeeded = true, Message = $"Order status updated to {newStatus} successfully." };
        }
        public async Task<bool> HasUserPurchasedProductAsync(string userId, Guid productId)
        {
            return await _unitOfWork.Repository<Order>().Query()
                .AnyAsync(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId) && o.OrderStatus == OrderStatus.Delivered);
        }
    }
}
