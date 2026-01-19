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
        private readonly IRepository<Cart> _repository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly ProductService _productService;

        public CartService(IRepository<Cart> repository, IRepository<CartItem> CartItemRepository, ProductService productService)
        {
            _repository = repository;
            _cartItemRepository = CartItemRepository;
            _productService = productService;
        }

        public async Task<OperationResult<OutputCartDto>> GetCartAsync(string userId)
        {
            var cart = await _repository.Query()
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
                return new OperationResult<OutputCartDto> { Succeeded = false, Message = "Cart not found." };

            var output = new OutputCartDto
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

            return new OperationResult<OutputCartDto>
            {
                Succeeded = true,
                Data = output
            };
        }
        public async Task<OperationResult> AddToCartAsync(string userId, InputAddToCartDto cartDto)
        {
            var productCount = await _productService.GetProductCountByIdAsync(cartDto.ProductId);

            if (productCount == 0)
                return new OperationResult { Succeeded = false, Message = "Product does not exist." };

            var cart = await _repository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _repository.AddAsync(cart);
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
            await _repository.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Item added to cart successfully." };
        }
        public async Task<OperationResult> UpdateCartAsync(string userId, InputAddToCartDto cartDto)
        {
            var cart = await _repository.Query()
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
                    await _repository.SaveChangesAsync();
                }

                return new OperationResult { Succeeded = true, Message = "Cart updated successfully." };
            }
            return new OperationResult { Succeeded = false, Message = "Product not found in cart." };
        }

        public async Task<OperationResult> RemoveFromCartAsync(string userId, Guid cartItemId)
        {
            var item = await _repository.Query()
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .FirstOrDefaultAsync(i => i.Id == cartItemId);

            if (item == null)
                return new OperationResult { Succeeded = false, Message = "Item not found in cart." };

            _cartItemRepository.Delete(item);

            var cart = await _repository.Query().FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart != null)
                cart.UpdatedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChangesAsync();

            return new OperationResult { Succeeded = true, Message = "Item removed from cart successfully." };
        }
        public async Task<OperationResult> DeleteCartAsync(string userId)
        {
            var cart = await _repository.Query()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
                return new OperationResult { Succeeded = false, Message = "Cart not found." };
            _cartItemRepository.RemoveRange(cart.Items);
            _repository.Delete(cart);
            await _repository.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Cart deleted successfully." };
        }
    }
}
