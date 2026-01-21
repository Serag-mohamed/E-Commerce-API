using E_Commerce.DTOs;
using E_Commerce.DTOs.InputDtos;
using E_Commerce.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthService _authService;

        public AccountService(UserManager<ApplicationUser> userManager, AuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<OperationResult<string>> RegisterAsync(InputRegisterDto registerDto)
        {

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new OperationResult<string> { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };

            await _userManager.AddToRoleAsync(user, "Customer");
            var token = await _authService.CreateTokenAsync(user, _userManager);

            return new OperationResult<string> { Succeeded = true, Data = token };
        }
        public async Task<OperationResult<string>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return new OperationResult<string>
                {
                    Succeeded = false,
                    Errors = new List<string> { "Email or Password is incorrect" }
                };

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordValid)
                return new OperationResult<string>
                {
                    Succeeded = false,
                    Errors = new List<string> { "Email or Password is incorrect" }
                };

            var token = await _authService.CreateTokenAsync(user, _userManager);

            return new OperationResult<string>
            {
                Succeeded = true,
                Data = token
            };
        }
    }
}
