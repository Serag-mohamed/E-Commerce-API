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
    public class CartController : ControllerBase
    {
        private readonly CartService _service;

        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public CartController(CartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetCart()
        {
            var cart = await _service.GetCartAsync(_userId);
            return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult> AddToCart(InputAddToCartDto cartDto)
        { 
            var result = await _service.AddToCartAsync(_userId, cartDto);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });
            return Ok(new {message = result.Message});
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCart(InputAddToCartDto cartDto)
        {
            var result = await _service.UpdateCartAsync(_userId, cartDto);
            if (!result.Succeeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> RemoveFromCart(Guid productId)
        {
            var result = await _service.RemoveFromCartAsync(_userId, productId);
            if (!result.Succeeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCart()
        {
            var result = await _service.DeleteCartAsync(_userId);
            if (!result.Succeeded)
                return BadRequest(new {message = result.Message});
            return Ok(new {message = result.Message});
        }
    }
}
