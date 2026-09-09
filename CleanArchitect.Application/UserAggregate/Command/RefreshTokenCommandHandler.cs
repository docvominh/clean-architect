using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RefreshTokenCommandHandler(
    IUserIdentityService users,
    IRefreshTokenRepository refreshTokens,
    AuthSessionService sessions,
    IRefreshTokenCookie refreshTokenCookie) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenValue = refreshTokenCookie.Read();

        if (string.IsNullOrEmpty(refreshTokenValue))
        {
            throw new UserAuthenticationException();
        }

        var token = await refreshTokens.FindAsync(refreshTokenValue, cancellationToken);

        if (token is null || !token.IsActive)
        {
            refreshTokenCookie.Clear();

            throw new UserAuthenticationException();
        }

        var user = await users.FindByIdAsync(token.UserId, cancellationToken);

        if (user is null)
        {
            refreshTokenCookie.Clear();

            throw new UserAuthenticationException();
        }

        var (response, newToken) = await sessions.CreateAsync(user, cancellationToken);
        token.RevokedAt = DateTimeOffset.UtcNow;
        token.ReplacedByToken = newToken.Token;
        await refreshTokens.SaveChangesAsync(cancellationToken);

        return response;
    }
}
