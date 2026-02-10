using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(CategoryService categoryService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<OutputCategoryDto>>> GetAll()
        {
            var categories = await categoryService.GetAllAsync();
            if (categories is null || categories.Count == 0)
                return NoContent();
            return Ok(categories);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<OutputCategoryDto>> GetById(Guid id)
        {
            var category = await categoryService.GetCategoryInfoByIdAsync(id);
            if (category is null)
                return NotFound(new { Message = "Category not found" });

            return Ok(category);
        }

        [AllowAnonymous]
        [HttpGet("{id}/products")]
        public async Task<ActionResult<OperationResult<CategoryWithProductsDto>>> GetProductsByCategoryId(Guid id, int pageNumber = 1, int pageSize = 20)
        {
            
            var result = await categoryService.GetProductsByCategoryIdAsync(id, pageNumber, pageSize);
            if (!result.IsSucceeded)
                return NoContent();
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Category>> Add(InputCategoryDto categoryDto)
        {
            var result = await categoryService.AddAsync(categoryDto);
            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(Guid id, OutputCategoryDto categoryDto)
        {
            var result = await categoryService.UpdateAsync(id, categoryDto);
            if (!result.IsSucceeded)
                return BadRequest(new { Message = result.Message });
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await categoryService.DeleteAsync(id);
            if (!result.IsSucceeded)
                return NotFound(new { Message = result.Message });
            return NoContent();
            
        }

    }
}
