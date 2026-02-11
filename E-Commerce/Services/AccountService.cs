using E_Commerce.DTOs.InputDtos;
using E_Commerce.DTOs.OutputDtos;
using E_Commerce.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class AccountService(UserManager<ApplicationUser> userManager, AuthService authService)
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, string role)
        {
            if (await userManager.FindByEmailAsync(registerDto.UserName) is not null)
                return new AuthResponseDto { Message = "Username is already registered" };

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new AuthResponseDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await userManager.AddToRoleAsync(user, role);

            var token = await authService.GenerateTokenAsync(user);
            var refreshToken = authService.GenerateRefreshToken(user.Id);

            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = token,
                RefreshToken = refreshToken.Token,
                Expiration = refreshToken.Expired,
                UserName = user.UserName,
                Email = user.Email
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.Email == loginDto.Username);
            if (user is null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResponseDto { Message = "Invalid username or password" };

            var token = await authService.GenerateTokenAsync(user);
            var refreshToken = authService.GenerateRefreshToken(user.Id);

            var expiredTokens = user.RefreshTokens.Where(rt => rt.IsExpired || rt.IsRevoked).ToList();
            if (expiredTokens.Count != 0)
                foreach (var expiredToken in expiredTokens)
                    user.RefreshTokens.Remove(expiredToken);

            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);

            var roles = await userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = token,
                RefreshToken = refreshToken.Token,
                Expiration = refreshToken.Expired,
                UserName = user.UserName,
                Email = user.Email,
                Roles = [.. roles]
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user is null)
                return new AuthResponseDto { Message = "Invalid refresh token" };

            var storedToken = user.RefreshTokens.Single(rt => rt.Token == refreshToken);
            if (!storedToken.IsActive)
                return new AuthResponseDto { Message = "Refresh token is expired or revoked" };

            storedToken.Revoked = DateTime.UtcNow;
            var expiredTokens = user.RefreshTokens.Where(rt => rt.IsExpired || rt.IsRevoked).ToList();
            if (expiredTokens.Count != 0)
                foreach (var expiredToken in expiredTokens)
                    user.RefreshTokens.Remove(expiredToken);

            var newRefreshToken = authService.GenerateRefreshToken(user.Id);
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var token = await authService.GenerateTokenAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                IsAuthenticated = true,
                Token = token,
                RefreshToken = newRefreshToken.Token,
                Expiration = newRefreshToken.Expired,
                UserName = user.UserName,
                Email = user.Email,
                Roles = [.. roles]
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var user = await userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
            if (user is null)
                return false;

            var storedToken = user.RefreshTokens.Single(rt => rt.Token == refreshToken);
            if (!storedToken.IsActive)
                return false;

            storedToken.Revoked = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            return true;
        }
    }
}
