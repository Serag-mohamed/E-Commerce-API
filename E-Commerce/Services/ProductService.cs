using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;
namespace E_Commerce.Services
{
    public class ProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<ProductImage> _imageRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductService(IRepository<Product> repository, IRepository<ProductImage> imageRepository, IRepository<Category> categoryRepository)
        {
            _repository = repository;
            _imageRepository = imageRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Product> AddAsync(InputProductDto productDto)
        {
            ValidateImages(productDto.Images);

            Product product = new Product()
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                DiscountPrice = productDto.DiscountPrice,
                Quantity = productDto.Quantity,
                CategoryId = productDto.CategoryId,
                ProductImages = productDto.Images.Select(img => new ProductImage()
                {
                    ImageUrl = img.ImageUrl,
                    IsMain = img.IsMain
                }).ToList()
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return product;
        }

        public async Task<OperationResult> UpdateAsync(Guid id, InputProductDto productDto)
        {
            var result = ValidateImages(productDto.Images);
            if (!result.Succeeded)
                return result;

            var IsExistsCategory = await _categoryRepository.GetByIdAsync(productDto.CategoryId);
            if (IsExistsCategory == null)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Category with ID {productDto.CategoryId} was not found."
                };

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Product with ID {id} was not found."
                };

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.DiscountPrice = productDto.DiscountPrice;
            product.Quantity = productDto.Quantity;
            product.CategoryId = productDto.CategoryId;

            var oldImages = _imageRepository.Query().Where(pi => pi.ProductId == id).ToList();
            if (oldImages.Any())
                _imageRepository.RemoveRange(oldImages);

            product.ProductImages = productDto.Images.Select(img => new ProductImage
            {
                ImageUrl = img.ImageUrl,
                IsMain = img.IsMain,
                ProductId = id
            }).ToList();

            _repository.Update(product);
            await _repository.SaveChangesAsync();

            return new OperationResult { Succeeded = true };
        }

        public async Task<OperationResult> DeleteAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return new OperationResult
                {
                    Succeeded = false,
                    Message = $"Product with ID {id} was not found."
                };

            _repository.Delete(product);
            await _repository.SaveChangesAsync();

            return new OperationResult { Succeeded = true };

        }
        private static OperationResult ValidateImages(IEnumerable<InputProductImageDto> imgsDto)
        {
            var mainImagesCount = imgsDto.Count(i => i.IsMain);
            var totalImages = imgsDto.Count();

            if (totalImages == 0)
                return new OperationResult { Succeeded = false, Message = "Product must have at least one image." };

            if (mainImagesCount == 0)
                return new OperationResult { Succeeded = false, Message = "Product must have a main image." };

            if (mainImagesCount > 1)
                return new OperationResult { Succeeded = false, Message = "Only one main image is allowed." };

            return new OperationResult { Succeeded = true };
        }

        public async Task<OutputProductDto?> GetProductInfoByIdAsync(Guid id)
        {
            return await _repository.Query()
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new OutputProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
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
                            UserName = r.User != null ? r.User.Name : "Guest",
                            Rate = r.Rate,
                            Comment = r.Comment,
                            ReviewDate = r.ReviewDate
                        }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<List<OutputProductListDto>> GetAll(int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize > 100 ? 100 : pageSize;
            pageSize = pageSize < 1 ? 20 : pageSize;
            int skipNumber = (pageNumber - 1) * pageSize;

            return await _repository.Query()
                .AsNoTracking()
                .OrderByDescending(p => p.TotalSalesCount)
                .Skip(skipNumber)
                .Take(pageSize)
                .Select(p => new OutputProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DiscountPrice = p.DiscountPrice,
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
            return await _repository.Query()
                .Where(p => p.Id == productId)
                .Select(p => p.Quantity)
                .FirstOrDefaultAsync();
        }

    }
}
