using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Enums;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(AccountService service) : ControllerBase
    {
        [HttpPost("register/{role}")]
        public async Task<ActionResult> Register(UserRole role, [FromBody] RegisterDto registerDto)
        {
            if (role is not ( UserRole.Customer or UserRole.Vendor ))
                return BadRequest("Invalid role specified. Only Customer or Vendor roles are allowed.");

            var result = await service.RegisterAsync(registerDto, role.ToString());
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken!, result.Expiration);

            return Ok(result);
        }

        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            var result = await service.RegisterAsync(registerDto, UserRole.Admin.ToString());
            if (!result.IsAuthenticated)
                return BadRequest(result.Message); 

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await service.LoginAsync(loginDto);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken!, result.Expiration);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is missing.");

            var result = await service.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken!, result.Expiration);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<OperationResult<string>>> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is missing.");

            var result = await service.RevokeTokenAsync(refreshToken);
            if (!result)
                return BadRequest("Invalid token");

            Response.Cookies.Delete("refreshToken");
            return Ok("Token revoked");
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime? expiration)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiration,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }


    }
}
