namespace CleanArchitect.Domain.UserAggregate;

public class RefreshToken : BaseEntity
{
    public required Guid UserId { get; set; }

    public required string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}
