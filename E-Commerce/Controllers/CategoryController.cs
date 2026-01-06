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

        [HttpPost]
        public async Task<ActionResult<Category>> Add(InputCategoryDto categoryDto)
        {
            Category category = await _service.AddAsync(categoryDto);
            return CreatedAtAction(nameof(GetById), new {id = category.Id}, category);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Category>> Update(Guid id, OutputCategoryDto categoryDto)
        {
            await _service.UpdateAsync(id, categoryDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
            
        }

    }
}
