using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RegisterCommandHandler(
    IUserIdentityService users,
    AuthSessionService sessions,
    IRefreshTokenRepository refreshTokens) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await users.CreateAsync(request.RegisterRequest, cancellationToken);
        await users.AddToRoleAsync(user.Id, "User", cancellationToken);
        var (response, _) = await sessions.CreateAsync(user, cancellationToken);
        await refreshTokens.SaveChangesAsync(cancellationToken);
        return response;
    }
}
