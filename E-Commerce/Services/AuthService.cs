using E_Commerce.Entities;
using E_Commerce.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace E_Commerce.Services
{
    public class AuthService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName!),
                new (ClaimTypes.Email, user.Email!),
                new (ClaimTypes.NameIdentifier, user.Id),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new ("DisplayName", user.DisplayName!)
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles) 
                authClaims.Add(new (ClaimTypes.Role, role));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: authClaims,
                signingCredentials: creds,
                expires: _jwtSettings.TokenExpirationDate
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string userId)
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expired = _jwtSettings.RefreshTokenExpirationDate,
                UserId = userId
            };
        }

    }
}
