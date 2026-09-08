namespace CleanArchitect.Domain.UserAggregate;

public class RefreshToken : BaseEntity
{
    public RefreshToken(Guid id, Guid createBy, Guid userId, string token, DateTimeOffset expiresAt) : base(id, createBy)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public Guid UserId { get; init; }

    public string Token { get; init; }

    public DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
