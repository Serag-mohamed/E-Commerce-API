using E_Commerce.DTOs;
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
         
        public async Task<OperationResult> UpdateAsync(Guid id, OutputCategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "ID mismatch"
                };

            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Category with ID: {id} was not found"
                };

            category.Name = categoryDto.Name;
            category.ParentCategoryId = categoryDto.ParentCategoryId;

            _repository.Update(category);
            await _repository.SaveChangesAsync();

            return new OperationResult
            {
                Succeeded = true,
                Message = "Category updated successfully"
            };
        }

        public async Task<OperationResult> DeleteAsync(Guid id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Category with ID: {id} was not found"
                };

            _repository.Delete(category);
            await _repository.SaveChangesAsync();

            return new OperationResult
            {
                Succeeded = true,
                Message = "Category deleted successfully"
            };
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
        public async Task<OperationResult<OutputCategoryWithProductsDto>> GetProductsByCategoryIdAsync(Guid categoryId, int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            pageSize = pageSize < 1 ? 20 : pageSize;

            int skipNumber = (pageNumber - 1) * pageSize;
            var category = await _repository.Query()
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(c => new OutputCategoryWithProductsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : string.Empty,
                    TotalProducts = c.Products.Count(),
                    Products = c.Products
                        .OrderByDescending(p => p.TotalSalesCount)
                        .Skip(skipNumber)
                        .Take(pageSize)
                        .Select(p => new OutputProductListDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            DiscountPrice = p.DiscountPrice,
                            CategoryName = p.Category.Name,
                            MainImageUrl = p.ProductImages
                                .Where(pi => pi.IsMain)
                                .Select(pi => pi.ImageUrl)
                                .FirstOrDefault() ?? string.Empty,
                            AverageRate = p.Reviews.Any() ? p.Reviews.Average(r => r.Rate) : 0,
                            ReviewsCount = p.Reviews.Count(),
                            Quantity = p.Quantity
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (category == null)
            {
                return new OperationResult<OutputCategoryWithProductsDto>
                {
                    Succeeded = false,
                    Message = "Category not found",
                    Data = null
                };
            }

            return new OperationResult<OutputCategoryWithProductsDto>
            {
                Succeeded = true,
                Message = "Category products retrieved successfully",
                Data = category
            };
        }


    }

}
