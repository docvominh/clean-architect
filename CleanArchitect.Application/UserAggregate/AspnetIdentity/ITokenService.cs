using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate.AspnetIdentity;

// Deliberately not the concrete ASP.NET Core Identity AppUser (which lives in Infrastructure) -
// Application must not depend on it. Just the fields GenerateAccessToken actually needs.
public record TokenSubject(Guid Id, string? Email, string? UserName, string? DisplayName);

public interface ITokenService
{
    (string AccessToken, DateTimeOffset ExpiresAt) GenerateAccessToken(TokenSubject user, IList<string> roles);

    RefreshToken GenerateRefreshToken(Guid userId);
}
