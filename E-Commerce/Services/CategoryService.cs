using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CategoryService(IUnitOfWork unitOfWork)
    {
        private IRepository<Category> Repository => unitOfWork.Repository<Category>();

        public async Task<OperationResult<Category>> AddAsync(InputCategoryDto categoryDto)
        {
            var existingCategory = await Repository.Query().IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Name == categoryDto.Name);
            if (existingCategory != null)
            {
                if (existingCategory.IsDeleted)
                {
                    existingCategory.IsDeleted = false;
                    existingCategory.DeletedAt = null;
                    existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;

                    Repository.Update(existingCategory);
                    await unitOfWork.SaveChangesAsync();
                    return new OperationResult<Category>
                    {
                        IsSucceeded = true,
                        Message = "The category has been successfully added",
                        Data = existingCategory
                    };
                }

                return new OperationResult<Category>
                {
                    IsSucceeded = false,
                    Message = "Category already exists"
                };

            }
            Category category = new Category()
            {
                Name = categoryDto.Name,
                ParentCategoryId = categoryDto.ParentCategoryId
            };
            await Repository.AddAsync(category);
            await unitOfWork.SaveChangesAsync();
            return new OperationResult<Category>
            {
                IsSucceeded = true,
                Message = "The category has been successfully added",
                Data = category
            };
        }
         
        public async Task<OperationResult> UpdateAsync(Guid id, OutputCategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = "ID mismatch"
                };

            var category = await Repository.GetByIdAsync(id);
            if (category == null)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Category with ID: {id} was not found"
                };

            category.Name = categoryDto.Name;
            category.ParentCategoryId = categoryDto.ParentCategoryId;

            Repository.Update(category);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult
            {
                IsSucceeded = true,
                Message = "Category updated successfully"
            };
        }

        public async Task<OperationResult> DeleteAsync(Guid id)
        {
            var category = await Repository.GetByIdAsync(id);
            if (category == null)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Category with ID: {id} was not found"
                };

            Repository.Delete(category);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult
            {
                IsSucceeded = true,
                Message = "Category deleted successfully"
            };
        }
        public async Task<List<OutputCategoryDto>?> GetAllAsync()
        {
            return await Repository.Query()
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
            return await Repository.Query()
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new OutputCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryId = c.ParentCategoryId
                }).FirstOrDefaultAsync();
        }
        public async Task<OperationResult<CategoryWithProductsDto>> GetProductsByCategoryIdAsync(Guid categoryId, int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            pageSize = pageSize < 1 ? 20 : pageSize;

            int skipNumber = (pageNumber - 1) * pageSize;
            var category = await Repository.Query()
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(c => new CategoryWithProductsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : string.Empty,
                    TotalProducts = c.Products.Count(),
                    Products = c.Products
                        .OrderByDescending(p => p.TotalSalesCount)
                        .Skip(skipNumber)
                        .Take(pageSize)
                        .Select(p => new ProductListDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            SalePrice = p.SalePrice,
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
                return new OperationResult<CategoryWithProductsDto>
                {
                    IsSucceeded = false,
                    Message = "Category not found",
                    Data = null
                };
            }

            return new OperationResult<CategoryWithProductsDto>
            {
                IsSucceeded = true,
                Message = "Category products retrieved successfully",
                Data = category
            };
        }


    }

}
