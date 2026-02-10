using E_Commerce.DTOs.InputDtos;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(ReviewService service) : ControllerBase
    {
        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            var result = await service.AddReviewAsync(UserId, reviewDto);
            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] ReviewDto reviewDto)
        {
            var result = await service.UpdateReviewAsync(reviewId, UserId, reviewDto);

            if (!result.IsSucceeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            bool isAdmin = User.IsInRole("Admin");
            var result = await service.DeleteReviewAsync(reviewId, UserId, isAdmin);
            if (!result.IsSucceeded)
            {
                if (result.Message!.Contains("not found"))
                    return NotFound(new { message = result.Message });
                if (result.Message.Contains("permission"))
                    return Forbid();
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }
    }
}
