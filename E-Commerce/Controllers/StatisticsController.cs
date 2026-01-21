using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _statisticsService.GetAdminStatsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor")]
        public async Task<IActionResult> GetVendorDashboard()
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _statisticsService.GetVendorStatsAsync(vendorId!);
            return Ok(result);
        }
    }
}
