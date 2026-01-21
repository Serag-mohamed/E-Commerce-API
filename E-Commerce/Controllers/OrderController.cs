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
    public class OrderController : ControllerBase
    {
        private readonly OrderService _service;
        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool _isAdmin => User.IsInRole("Admin");
        public OrderController(OrderService service)
        {
            _service = service;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout(CheckoutDto checkoutDto)
        {
            var result = await _service.CheckOutAsync(_userId, checkoutDto);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { orderId = result.Data, message = "Order placed successfully!" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderDetails(Guid id)
        {
            var result = await _service.GetOrderDetaialsAsync(id, _userId, _isAdmin);
            if (!result.Succeeded)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetUserOrders()
        {
            var result = await _service.GetOrdersAsync(_userId, _isAdmin);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<ActionResult> UpdateOrderStatus(Guid orderId, [FromBody] OrderStatus newStatus)
        {
            var result = await _service.UpdateOrderStatusAsync(orderId, newStatus);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}