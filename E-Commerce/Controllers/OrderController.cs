using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Enums;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(OrderService service) : ControllerBase
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool IsAdmin => User.IsInRole("Admin");

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout(CheckoutDto checkoutDto)
        {
            var result = await service.CheckOutAsync(UserId, checkoutDto);

            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { orderId = result.Data, message = "Order placed successfully!" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderDetails(Guid id)
        {
            var result = await service.GetOrderDetaialsAsync(id, UserId, IsAdmin);
            if (!result.IsSucceeded)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetUserOrders()
        {
            var result = await service.GetOrdersAsync(UserId, IsAdmin);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<ActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderStatus newStatus)
        {
            var result = await service.UpdateOrderStatusAsync(orderId, newStatus);

            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}