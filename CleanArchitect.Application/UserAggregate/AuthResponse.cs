namespace CleanArchitect.Application.UserAggregate;

public class AuthResponse
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}