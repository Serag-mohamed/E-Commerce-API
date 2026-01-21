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
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _service;
        private string _userId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        public ReviewController(ReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] InputReviewDto reviewDto)
        {
            var result = await _service.AddReviewAsync(_userId, reviewDto);
            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] InputReviewDto reviewDto)
        {
            var result = await _service.UpdateReviewAsync(reviewId, _userId, reviewDto);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            bool isAdmin = User.IsInRole("Admin");
            var result = await _service.DeleteReviewAsync(reviewId, _userId, isAdmin);
            if (!result.Succeeded)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(new { message = result.Message });
                if (result.Message.Contains("permission"))
                    return Forbid();
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }
    }
}
