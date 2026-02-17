using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Services
{
    public class ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
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

            var mapResult = new OperationResult<Product>();

            if (existingProduct != null)
            {
                if (existingProduct.IsDeleted)
                {
                     mapResult = await MapToProductAsync(productDto, userId, existingProduct, true);

                    if (!mapResult.IsSucceeded)
                        return new OperationResult<Product> { IsSucceeded = false, Message = mapResult.Message };

                    existingProduct = mapResult.Data!;

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

            var product = new Product();
            mapResult = await MapToProductAsync(productDto, userId, product);
            if (!mapResult.IsSucceeded)
                return new OperationResult<Product> { IsSucceeded = false, Message = mapResult.Message };

            product = mapResult.Data!;

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

            var mapResult = await MapToProductAsync(productDto, userId, product);
            if (!mapResult.IsSucceeded)
                return new OperationResult { IsSucceeded = false, Message = mapResult.Message };

            product = mapResult.Data!;
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
        
        private async Task<OperationResult<Product>> MapToProductAsync(InputProductDto productDto, string vendorId, Product product, bool isDeleted = false)
        {
            
            if (isDeleted)
            {
                product.IsDeleted = false;
                product.DeletedAt = null;
            }


            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.SalePrice = productDto.SalePrice;
            product.Quantity = productDto.Quantity;
            product.CategoryId = productDto.CategoryId;
            product.VendorId = vendorId;

            if (product.ProductImages.Count > 0)
                ImageRepository.RemoveRange(product.ProductImages);

            try
            {
                var imageTasks = productDto.Images.Select(img => HandleProductImageAsync(img, product.Id)).ToList();
                var images = await Task.WhenAll(imageTasks); 
                product.ProductImages = images.ToList();
            }
            catch (Exception ex)
            {
                return new OperationResult<Product> { IsSucceeded = false, Errors = [ex.Message] };
            }



            return new OperationResult<Product>
            {
                IsSucceeded = true,
                Data = product
            };
            
        }
        private async Task<ProductImage> HandleProductImageAsync(InputProductImageDto imgDto, Guid? productId = null)
        {   
            var extension = Path.GetExtension(imgDto.ImageFile.FileName).ToLower();
            var imageName = Guid.NewGuid().ToString() + extension;
            var uploadFolder = Path.Combine(env.WebRootPath, "Images");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, imageName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imgDto.ImageFile.CopyToAsync(stream);
            }

            if (productId.HasValue)
            {
                return new ProductImage
                {
                    ImageUrl = imageName,
                    IsMain = imgDto.IsMain,
                    ProductId = productId.Value
                };
            }

            return new ProductImage
            {
                ImageUrl = imageName,
                IsMain = imgDto.IsMain
            };
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

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            foreach (var img in imgsDto)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(img.ImageFile.FileName).ToLower()))
                {
                    return new OperationResult { IsSucceeded = false, Message = $"The file '{img.ImageFile.FileName}' is invalid. Allowed formats are: .jpg, .jpeg, .png, .gif" };
                }   
            }
            
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
