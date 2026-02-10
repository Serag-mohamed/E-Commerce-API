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
    public class ProductController(ProductService service) : ControllerBase
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool IsAdmin => User.IsInRole("Admin");

        [AllowAnonymous]
        [HttpGet] 
        public async Task<ActionResult> GetAll([FromQuery]int pageNumber = 1 , [FromQuery]int pageSize = 20)
        {

            var products = await service.GetAll(pageNumber, pageSize);

            return Ok(products); 
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var product = await service.GetProductInfoByIdAsync(id);
            if (product is null) 
                return NotFound();
            return Ok(product);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPost]
        public async Task<ActionResult<Product>> Add(InputProductDto productDto)
        {
            var result = await service.AddAsync(productDto, UserId);
            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            var displayedProduct = await service.GetProductInfoByIdAsync(result.Data!.Id);
            if (displayedProduct == null) 
                return NotFound();

            return CreatedAtAction(nameof(GetById), new { id = displayedProduct.Id }, displayedProduct);
        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, InputProductDto productDto)
        {
            var result = await service.UpdateAsync(id, productDto, UserId, IsAdmin);

            if (!result.IsSucceeded)
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
            var result = await service.DeleteAsync(id, UserId, IsAdmin);
            if (!result.IsSucceeded)
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
