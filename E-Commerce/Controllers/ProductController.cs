using E_Commerce.DTOs.InputDtos;
using E_Commerce.Entities;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductController(ProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery]int pageNumber = 1 , [FromQuery]int pageSize = 10)
        {
            if(pageSize <= 0 )
                pageSize = 10;

            if(pageSize > 50) 
                pageSize = 50;

            if (pageNumber <= 0)
                pageNumber = 1;

            var products = await _service.GetAll(pageNumber, pageSize);

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            var product = await _service.GetProductInfoByIdAsync(id);
            if (product is null) 
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Add(InputProductDto productDto)
        {
            Product product = await _service.AddAsync(productDto);

            var displayedProduct = await _service.GetProductInfoByIdAsync(product.Id);
            return CreatedAtAction(nameof(GetById), new { id = displayedProduct?.Id }, displayedProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, InputProductDto productDto)
        {
            await _service.UpdateAsync(id, productDto);
            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }


    }
}
