using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoryController(CategoryService categoryService)
        {
            _service = categoryService;
        }
          
        [HttpGet]
        public async Task<ActionResult<List<OutputCategoryDto>>> GetAll()
        {
            var categories = await _service.GetAllAsync();
            if (categories is null || categories.Count == 0)
                return NoContent();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OutputCategoryDto>> GetById(Guid id)
        {
            var category = await _service.GetCategoryInfoByIdAsync(id);
            if (category is null)
                return NotFound(new { Message = "Category not found" });

            return Ok(category);
        }
        [HttpGet("{id}/products")]
        public async Task<ActionResult<OperationResult<OutputCategoryWithProductsDto>>> GetProductsByCategoryId(Guid id, int pageNumber = 1, int pageSize = 20)
        {
            
            var result = await _service.GetProductsByCategoryIdAsync(id, pageNumber, pageSize);
            if (!result.Succeeded)
                return NoContent();
            return Ok(result.Data);
        }
        [HttpPost]
        public async Task<ActionResult<Category>> Add(InputCategoryDto categoryDto)
        {
            Category category = await _service.AddAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new {id = category.Id}, category);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(Guid id, OutputCategoryDto categoryDto)
        {
            var result = await _service.UpdateAsync(id, categoryDto);
            if (!result.Succeeded)
                return BadRequest(new { Message = result.Message });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Succeeded)
                return NotFound(new { Message = result.Message });
            return NoContent();
            
        }

    }
}
