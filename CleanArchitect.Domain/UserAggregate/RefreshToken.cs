namespace CleanArchitect.Domain.UserAggregate;

public class RefreshToken
{
    public int Id { get; private set; }

    public required Guid UserId { get; set; }

    public required string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTimeOffset ModifiedOn { get; set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}