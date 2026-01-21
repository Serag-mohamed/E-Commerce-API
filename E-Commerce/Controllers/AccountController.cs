using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _service;

        public AccountController(AccountService service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<OperationResult<string>>> Register(InputRegisterDto registerDto)
        {
            var result = await _service.RegisterAsync(registerDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<OperationResult<string>>> Login(LoginDto loginDto)
        {
            var result = await _service.LoginAsync(loginDto);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<ActionResult<OperationResult<string>>> Logout()
        {
            return Ok(new OperationResult<string>
            {
                Succeeded = true,
                Message = "Logged out successfully. Please delete the token from client storage."
            });
        }
    }
}
