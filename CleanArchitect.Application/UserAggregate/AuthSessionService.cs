using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate;

public sealed class AuthSessionService(
    IUserIdentityService users,
    ITokenService tokens,
    IRefreshTokenRepository refreshTokens,
    IRefreshTokenCookie refreshTokenCookie)
{
    // Issues both tokens, persists and cookies the refresh token, and returns the new refresh
    // token so callers can link it to the token it replaces (e.g. RefreshTokenCommandHandler).
    public async Task<(AuthResponse Response, RefreshToken RefreshToken)> CreateAsync(TokenSubject user, CancellationToken cancellationToken)
    {
        var roles = await users.GetRolesAsync(user.Id, cancellationToken);
        var (accessToken, expiresAt) = tokens.GenerateAccessToken(user, roles);
        var refreshToken = tokens.GenerateRefreshToken(user.Id);
        refreshTokens.Add(refreshToken);
        refreshTokenCookie.Write(refreshToken.Token, refreshToken.ExpiresAt);
        return (new AuthResponse { AccessToken = accessToken, ExpiresAt = expiresAt }, refreshToken);
    }
}
