namespace E_Commerce.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Token { get; set; } = null!;
        public required DateTime Expired { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expired;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;

        public required string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
