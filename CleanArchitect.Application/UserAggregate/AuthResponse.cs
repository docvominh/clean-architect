namespace CleanArchitect.Application.UserAggregate;

public sealed record AuthResponse
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
