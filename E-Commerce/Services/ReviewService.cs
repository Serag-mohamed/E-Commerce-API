using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using E_Commerce.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ReviewService(IUnitOfWork unitOfWork, ProductService productService, OrderService orderService)
    {
        private IRepository<Review> Repository => unitOfWork.Repository<Review>();

        public async Task<OperationResult<Guid>> AddReviewAsync(string userId,ReviewDto reviewDto)
        {
            var productExists = await productService.IsProductExistsAsync(reviewDto.ProductId);
            if (!productExists)
            {
                return new OperationResult<Guid>
                {
                    IsSucceeded = false,
                    Message = "Product not found."
                };
            }
            var alreadyReviewed = await Repository.Query().AnyAsync(r => r.ProductId == reviewDto.ProductId && r.UserId == userId);
            if (alreadyReviewed)
            {
                return new OperationResult<Guid>
                {
                    IsSucceeded = false,
                    Message = "You have already reviewed this product."
                };
            }
            var hasPurchased = await orderService.HasUserPurchasedProductAsync(userId, reviewDto.ProductId);
            if (!hasPurchased)
            {
                return new OperationResult<Guid>
                {
                    IsSucceeded = false,
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
            await unitOfWork.SaveChangesAsync();

            return new OperationResult<Guid>
            {
                IsSucceeded = true,
                Message = "Review added successfully.",
                Data = review.Id
            };
        }

        public async Task<OperationResult> UpdateReviewAsync(Guid reviewId, string userId, ReviewDto reviewDto)
        {
            var review = await Repository.GetByIdAsync(reviewId);

            if (review == null)
                return new OperationResult { IsSucceeded = false, Message = "Review not found." };

            if (review.UserId != userId)
                return new OperationResult { IsSucceeded = false, Message = "You can only update your own reviews." };

            review.Rate = reviewDto.Rate;
            review.Comment = reviewDto.Comment;
            review.ReviewDate = DateTime.UtcNow;

            Repository.Update(review);
            await unitOfWork.SaveChangesAsync();

            return new OperationResult { IsSucceeded = true, Message = "Review updated successfully." };
        }

        public async Task<OperationResult> DeleteReviewAsync(Guid reviewId, string userId, bool isAdmin)
        {
            var review = await Repository.GetByIdAsync(reviewId);
            if (review == null)
                return new OperationResult { IsSucceeded = false, Message = "Review not found." };
            if (review.UserId != userId && !isAdmin)
                return new OperationResult { IsSucceeded = false, Message = "Forbidden: You don't have permission to delete this review." };

            Repository.Delete(review);
            await unitOfWork.SaveChangesAsync();
            return new OperationResult { IsSucceeded = true, Message = "Review deleted successfully." };
        }
    }
}
