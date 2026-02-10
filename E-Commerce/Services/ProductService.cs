using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Services
{
    public class ProductService(IUnitOfWork unitOfWork)
    {
        private IRepository<Product> Repository => unitOfWork.Repository<Product>();
        private IRepository<ProductImage> ImageRepository => unitOfWork.Repository<ProductImage>();
        private IRepository<Category> CategoryRepository => unitOfWork.Repository<Category>();
        private IRepository<CartItem> CartItemRepo => unitOfWork.Repository<CartItem>();

        public async Task<OperationResult<Product>> AddAsync(InputProductDto productDto, string userId)
        {
            var result = ValidateImages(productDto.Images);
            if (!result.IsSucceeded)
                return new OperationResult<Product> { IsSucceeded = false, Message = result.Message };

            var existingProduct = await Repository.Query()
                .IgnoreQueryFilters()
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Name == productDto.Name && p.VendorId == userId);

            if (existingProduct != null)
            {
                if (existingProduct.IsDeleted)
                {
                    existingProduct.IsDeleted = false;
                    existingProduct.DeletedAt = null;

                    existingProduct.Description = productDto.Description;
                    existingProduct.Price = productDto.Price;
                    existingProduct.SalePrice = productDto.SalePrice;
                    existingProduct.Quantity = productDto.Quantity;
                    existingProduct.CategoryId = productDto.CategoryId;

                    ImageRepository.RemoveRange(existingProduct.ProductImages);
                    existingProduct.ProductImages = productDto.Images.Select(img => new ProductImage
                    {
                        ImageUrl = img.ImageUrl,
                        IsMain = img.IsMain
                    }).ToList();

                    await unitOfWork.SaveChangesAsync();

                    return new OperationResult<Product>
                    {
                        IsSucceeded = true,
                        Message = "Product restored and updated successfully",
                        Data = existingProduct
                    };
                }

                return new OperationResult<Product> { IsSucceeded = false, Message = "Product already exists." };
            }

            Product product = new ()
            {
                Name = productDto.Name,
                Description = productDto.Description,
                VendorId = userId,
                Price = productDto.Price,
                SalePrice = productDto.SalePrice,
                Quantity = productDto.Quantity,
                CategoryId = productDto.CategoryId,
                ProductImages = productDto.Images.Select(img => new ProductImage()
                {
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            };

            await Repository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult<Product> { IsSucceeded = true, Data = product };
        }

        public async Task<OperationResult> UpdateAsync(Guid id, InputProductDto productDto, string userId, bool isAdmin)
        {
            var result = ValidateImages(productDto.Images);
            if (!result.IsSucceeded)
                return result;

            var IsExistsCategory = await CategoryRepository.GetByIdAsync(productDto.CategoryId);
            if (IsExistsCategory == null)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Category with ID {productDto.CategoryId} was not found."
                };

            var product = await Repository.Query().Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Product with ID {id} was not found."
                };

            if (!isAdmin && product.VendorId != userId)
                return new OperationResult { IsSucceeded = false, Message = "Forbidden: You are not the owner of this product." };

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.SalePrice = productDto.SalePrice;
            product.Quantity = productDto.Quantity;
            product.CategoryId = productDto.CategoryId;

            if (product.ProductImages.Any())
                ImageRepository.RemoveRange(product.ProductImages);

            product.ProductImages = productDto.Images.Select(img => new ProductImage
            {
                ImageUrl = img.ImageUrl,
                IsMain = img.IsMain,
                ProductId = id
            }).ToList();

            Repository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult { IsSucceeded = true };
        }

        public async Task<OperationResult> DeleteAsync(Guid id, string userId, bool isAdmin)
        {
            var product = await Repository.GetByIdAsync(id);
            if (product == null)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = $"Product with ID {id} was not found."
                };

            if (product.VendorId != userId && !isAdmin)
                return new OperationResult
                {
                    IsSucceeded = false,
                    Message = "Forbidden: You don't have permission to delete this product."
                };
            Repository.Delete(product);
            var cartItems = await CartItemRepo.Query().Where(ci => ci.ProductId == id).ToListAsync();
            CartItemRepo.RemoveRange(cartItems);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult { IsSucceeded = true };

        }
        private static OperationResult ValidateImages(IEnumerable<InputProductImageDto> imgsDto)
        {
            var mainImagesCount = imgsDto.Count(i => i.IsMain);
            var totalImages = imgsDto.Count();

            if (totalImages == 0)
                return new OperationResult { IsSucceeded = false, Message = "Product must have at least one image." };

            if (mainImagesCount == 0)
                return new OperationResult { IsSucceeded = false, Message = "Product must have a main image." };

            if (mainImagesCount > 1)
                return new OperationResult { IsSucceeded = false, Message = "Only one main image is allowed." };

            return new OperationResult { IsSucceeded = true };
        }

        public async Task<OutputProductDto?> GetProductInfoByIdAsync(Guid id)
        {
            return await Repository.Query()
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new OutputProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    Quantity = p.Quantity,
                    CategoryName = p.Category != null ? p.Category.Name : "General",

                    Images = p.ProductImages
                        .OrderByDescending(i => i.IsMain)
                        .Select(i => new OutputProductImagesDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            IsMain = i.IsMain
                        }).ToList(),

                    Reviews = p.Reviews
                        .OrderByDescending(r => r.ReviewDate)
                        .Select(r => new OutputReviewDto
                        {
                            Id = r.Id,
                            UserName = r.User != null ? r.User.DisplayName : "Guest",
                            Rate = r.Rate,
                            Comment = r.Comment,
                            ReviewDate = r.ReviewDate
                        }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<ProductListDto>> GetAll(int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            pageSize = pageSize < 1 ? 20 : pageSize;
            int skipNumber = (pageNumber - 1) * pageSize;

            return await Repository.Query()
                .AsNoTracking()
                .OrderByDescending(p => p.TotalSalesCount)
                .Skip(skipNumber)
                .Take(pageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    SalePrice = p.SalePrice,
                    Quantity = p.Quantity,
                    CategoryName = p.Category.Name,

                    MainImageUrl = p.ProductImages
                        .Where(img => img.IsMain)
                        .Select(img => img.ImageUrl)
                        .FirstOrDefault() ?? "default-image-url.png",

                    AverageRate = p.Reviews.Any() ? p.Reviews.Average(r => r.Rate) : 0,
                    ReviewsCount = p.Reviews.Count()
                })
                .ToListAsync();
        }
        public async Task<int> GetProductCountByIdAsync(Guid productId)
        {
            return await Repository.Query()
                .Where(p => p.Id == productId)
                .Select(p => p.Quantity)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsProductExistsAsync(Guid productId)
        {
            return await Repository.Query()
                .AnyAsync(p => p.Id == productId);
        }
    }
}
