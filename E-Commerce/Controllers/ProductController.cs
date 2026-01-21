using E_Commerce.DTOs.InputDtos;
using E_Commerce.Entities;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;
        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool _isAdmin => User.IsInRole("Admin");

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet] 
        public async Task<ActionResult> GetAll([FromQuery]int pageNumber = 1 , [FromQuery]int pageSize = 20)
        {

            var products = await _service.GetAll(pageNumber, pageSize);

            return Ok(products); 
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var product = await _service.GetProductInfoByIdAsync(id);
            if (product is null) 
                return NotFound();
            return Ok(product);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        public async Task<ActionResult<Product>> Add(InputProductDto productDto)
        {
            var result = await _service.AddAsync(productDto, _userId);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            var displayedProduct = await _service.GetProductInfoByIdAsync(result.Data!.Id);
            if (displayedProduct == null) 
                return NotFound();

            return CreatedAtAction(nameof(GetById), new { id = displayedProduct.Id }, displayedProduct);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, InputProductDto productDto)
        {
            var result = await _service.UpdateAsync(id, productDto, _userId, _isAdmin);

            if (!result.Succeeded)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { message = result.Message });

                if (result.Message.Contains("Forbidden") || result.Message.Contains("owner"))
                    return Forbid();

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id, _userId, _isAdmin);
            if (!result.Succeeded)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { message = result.Message });

                if (result.Message.Contains("Forbidden"))
                    return Forbid();

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }


    }
}
