using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ReviewService
    {
        private readonly IRepository<Review> _repository;
        private readonly ProductService _productService;
        private readonly OrderService _orderService;

        public ReviewService(IRepository<Review> repository, ProductService productService, OrderService orderService)
        {
            _repository = repository;
            _productService = productService;
            _orderService = orderService;
        }

        public async Task<OperationResult<Guid>> AddReviewAsync(string userId,InputReviewDto reviewDto)
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
            var alreadyReviewed = await _repository.Query().AnyAsync(r => r.ProductId == reviewDto.ProductId && r.UserId == userId);
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
            await _repository.AddAsync(review);
            await _repository.SaveChangesAsync();

            return new OperationResult<Guid>
            {
                Succeeded = true,
                Message = "Review added successfully.",
                Data = review.Id
            };
        }

        public async Task<OperationResult> UpdateReviewAsync(Guid reviewId, string userId, InputReviewDto reviewDto)
        {
            var review = await _repository.GetByIdAsync(reviewId);

            if (review == null)
                return new OperationResult { Succeeded = false, Message = "Review not found." };

            if (review.UserId != userId)
                return new OperationResult { Succeeded = false, Message = "You can only update your own reviews." };

            review.Rate = reviewDto.Rate;
            review.Comment = reviewDto.Comment;
            review.ReviewDate = DateTime.UtcNow;

            _repository.Update(review);
            await _repository.SaveChangesAsync();

            return new OperationResult { Succeeded = true, Message = "Review updated successfully." };
        }

        public async Task<OperationResult> DeleteReviewAsync(Guid reviewId, string userId)
        {
            var review = await _repository.GetByIdAsync(reviewId);
            if (review == null)
                return new OperationResult { Succeeded = false, Message = "Review not found." };
            if (review.UserId != userId)
                return new OperationResult { Succeeded = false, Message = "You can only delete your own reviews." };
            _repository.Delete(review);
            await _repository.SaveChangesAsync();
            return new OperationResult { Succeeded = true, Message = "Review deleted successfully." };
        }
    }
}
