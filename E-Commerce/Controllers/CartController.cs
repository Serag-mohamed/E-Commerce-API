using E_Commerce.DTOs.InputDtos;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(CartService service) : ControllerBase
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<ActionResult> GetCart()
        {
            var cart = await service.GetCartAsync(UserId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(AddToCartDto cartDto)
        { 
            var result = await service.AddToCartAsync(UserId, cartDto);
            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });
            return Ok(new {message = result.Message});
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCart(AddToCartDto cartDto)
        {
            var result = await service.UpdateCartAsync(UserId, cartDto);
            if (!result.IsSucceeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> RemoveFromCart(Guid productId)
        {
            var result = await service.RemoveFromCartAsync(UserId, productId);
            if (!result.IsSucceeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCart()
        {
            var result = await service.DeleteCartAsync(UserId);
            if (!result.IsSucceeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }
    }
}
