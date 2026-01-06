using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CategoryService
    {
        private readonly IRepository<Category> _repository;

        public CategoryService(IRepository<Category> repository) 
        {
            _repository = repository;
        }

        public async Task<Category> AddAsync(InputCategoryDto categoryDto)
        {
            Category category = new Category()
            {
                Name = categoryDto.Name,
                ParentCategoryId = categoryDto.ParentCategoryId
            };
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Guid id, OutputCategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
                throw new ArgumentException("ID mismatch");

            var category = await _repository.GetByIdAsync(id) ?? 
                throw new KeyNotFoundException($"Category with ID: {id} was not found");

            category.Name = categoryDto.Name;
            category.ParentCategoryId = categoryDto.ParentCategoryId;

            _repository.Update(category);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id) ??
                throw new KeyNotFoundException($"Category with ID: {id} was not found");

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
        public async Task<List<OutputCategoryDto>?> GetAllAsync()
        {
            return await _repository.Query()
                .AsNoTracking()
                .Select(c => new OutputCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                }).ToListAsync();
        }
        public async Task<OutputCategoryDto?> GetCategoryInfoByIdAsync(Guid id)
        {
            return await _repository.Query()
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new OutputCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                }).FirstOrDefaultAsync();
        }
         

    }

}
