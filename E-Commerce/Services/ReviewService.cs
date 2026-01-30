using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;

        private IRepository<Review> Repository => _unitOfWork.Repository<Review>();

        public ReviewService(IUnitOfWork unitOfWork, ProductService productService, OrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<OperationResult<Guid>> AddReviewAsync(string userId,ReviewDto reviewDto)
        {
            var productExists = await _productService.IsProductExistsAsync(reviewDto.ProductId);
            if (!productExists)
            {
                return new OperationResult<Guid>
                {
                    Succeeded = false,
                    Message = "Product not found."
                };
            }
            var alreadyReviewed = await Repository.Query().AnyAsync(r => r.ProductId == reviewDto.ProductId && r.UserId == userId);
            if (alreadyReviewed)
            {
                return new OperationResult<Guid>
                {
                    Succeeded = false,
                    Message = "You have already reviewed this product."
                };
            }
            var hasPurchased = await _orderService.HasUserPurchasedProductAsync(userId, reviewDto.ProductId);
            if (!hasPurchased)
            {
                return new OperationResult<Guid>
                {
                    Succeeded = false,
                    Message = "You can only review products you have purchased."
                };
            }
            var review = new Review
            {
                ProductId = reviewDto.ProductId,
                UserId = userId,
                Rate = reviewDto.Rate,
                Comment = reviewDto.Comment
            };
            await Repository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new OperationResult<Guid>
            {
                Succeeded = true,
                Message = "Review added successfully.",
                Data = review.Id
            };
        }

        public async Task<OperationResult> UpdateReviewAsync(Guid reviewId, string userId, ReviewDto reviewDto)
        {
            var review = await Repository.GetByIdAsync(reviewId);

            if (review == null)
                return new OperationResult { Succeeded = false, Message = "Review not found." };

            if (review.UserId != userId)
                return new OperationResult { Succeeded = false, Message = "You can only update your own reviews." };

            review.Rate = reviewDto.Rate;
            review.Comment = reviewDto.Comment;
            review.ReviewDate = DateTime.UtcNow;

            Repository.Update(review);
            await _unitOfWork.SaveChangesAsync();

            return new OperationResult { Succeeded = true, Message = "Review updated successfully." };
        }

        public async Task<OperationResult> DeleteReviewAsync(Guid reviewId, string userId, bool isAdmin)
        {
            var review = await Repository.GetByIdAsync(reviewId);
            if (review == null)
                return new OperationResult { Succeeded = false, Message = "Review not found." };
            if (review.UserId != userId && !isAdmin)
                return new OperationResult { Succeeded = false, Message = "Forbidden: You don't have permission to delete this review." };

            Repository.Delete(review);
            await _unitOfWork.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Review deleted successfully." };
        }
    }
}
