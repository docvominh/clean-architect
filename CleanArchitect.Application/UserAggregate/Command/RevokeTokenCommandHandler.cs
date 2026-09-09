using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RevokeTokenCommandHandler(
    IRefreshTokenRepository refreshTokens,
    IRefreshTokenCookie refreshTokenCookie) : IRequestHandler<RevokeTokenCommand>
{
    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenValue = refreshTokenCookie.Read();

        if (!string.IsNullOrEmpty(refreshTokenValue))
        {
            var token = await refreshTokens.FindAsync(refreshTokenValue, cancellationToken);

            if (token is not null && token.UserId == request.UserId && token.IsActive)
            {
                token.RevokedAt = DateTimeOffset.UtcNow;
                await refreshTokens.SaveChangesAsync(cancellationToken);
            }
        }

        refreshTokenCookie.Clear();
    }
}
