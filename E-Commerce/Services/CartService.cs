using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductService _productService;

        private IRepository<Cart> Repository => _unitOfWork.Repository<Cart>();
        private IRepository<CartItem> CartItemRepository => _unitOfWork.Repository<CartItem>();
        public CartService(IUnitOfWork unitOfWork, ProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        public async Task<OperationResult<CartDto>> GetCartAsync(string userId)
        {
            var cart = await Repository.Query()
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new OperationResult<CartDto> { Succeeded = false, Message = "Cart not found." };

            var output = new CartDto
            {
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ProductImageUrl = i.Product.ProductImages
                        .FirstOrDefault(img => img.IsMain)!.ImageUrl,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            return new OperationResult<CartDto>
            {
                Succeeded = true,
                Data = output
            };
        }
        public async Task<OperationResult> AddToCartAsync(string userId, AddToCartDto cartDto)
        {
            var productCount = await _productService.GetProductCountByIdAsync(cartDto.ProductId);

            if (productCount == 0)
                return new OperationResult { Succeeded = false, Message = "Product does not exist." };

            var cart = await Repository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await Repository.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == cartDto.ProductId);

            if (existingItem != null)
            {
                if (productCount < (existingItem.Quantity + cartDto.Quantity))
                    return new OperationResult { Succeeded = false, Message = $"Cannot add more items. Only {productCount} available in stock." };

                existingItem.Quantity += cartDto.Quantity;
            }
            else
            {
                if (productCount < cartDto.Quantity)
                    return new OperationResult { Succeeded = false, Message = $"Insufficient stock. Only {productCount} available." };

                cart.Items.Add(new CartItem
                {
                    ProductId = cartDto.ProductId,
                    Quantity = cartDto.Quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Item added to cart successfully." };
        }
        public async Task<OperationResult> UpdateCartAsync(string userId, AddToCartDto cartDto)
        {
            var cart = await Repository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var productCount = await _productService.GetProductCountByIdAsync(cartDto.ProductId);
            if (productCount == 0)
                return new OperationResult { Succeeded = false, Message = "Product does not exist." };
            if (productCount < cartDto.Quantity)
                return new OperationResult { Succeeded = false, Message = $"Sorry, only {productCount} units are available in stock." };

            if (cart == null)
            {
                await AddToCartAsync(userId, cartDto);
                return new OperationResult { Succeeded = true, Message = "Cart created and item added successfully." };
            }
            var existingItem = cart.Items!.FirstOrDefault(i => i.ProductId == cartDto.ProductId);
            if (existingItem != null)
            {
                if (cartDto.Quantity <= 0)
                    await RemoveFromCartAsync(userId, existingItem.Id);
                else
                {
                    existingItem.Quantity = cartDto.Quantity;
                    cart.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync();
                }

                return new OperationResult { Succeeded = true, Message = "Cart updated successfully." };
            }
            return new OperationResult { Succeeded = false, Message = "Product not found in cart." };
        }

        public async Task<OperationResult> RemoveFromCartAsync(string userId, Guid cartItemId)
        {
            var item = await Repository.Query()
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .FirstOrDefaultAsync(i => i.Id == cartItemId);

            if (item == null)
                return new OperationResult { Succeeded = false, Message = "Item not found in cart." };

            CartItemRepository.Delete(item);

            var cart = await Repository.Query().FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
                cart.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new OperationResult { Succeeded = true, Message = "Item removed from cart successfully." };
        }
        public async Task<OperationResult> DeleteCartAsync(string userId)
        {
            var cart = await Repository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                return new OperationResult { Succeeded = false, Message = "Cart not found." };
            CartItemRepository.RemoveRange(cart.Items);
            Repository.Delete(cart);
            await _unitOfWork.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Cart deleted successfully." };
        }
    }
}
