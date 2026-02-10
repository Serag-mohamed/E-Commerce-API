namespace E_Commerce.Settings
{
    public class JwtSettings
    {
        public required string Key { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required int TokenDurationInMinutes { get; init; }
        public required int RefreshTokenDurationInDays { get; init; }
        public DateTime TokenExpirationDate => DateTime.UtcNow.AddMinutes(TokenDurationInMinutes);
        public DateTime RefreshTokenExpirationDate => DateTime.UtcNow.AddDays(RefreshTokenDurationInDays);
    }
}