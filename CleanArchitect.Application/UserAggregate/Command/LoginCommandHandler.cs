using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class LoginCommandHandler(
    IUserIdentityService users,
    AuthSessionService sessions,
    IRefreshTokenRepository refreshTokens) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.FindByEmailAsync(request.LoginRequest.Email, cancellationToken);
        if (user is null || !await users.CheckPasswordAsync(user.Id, request.LoginRequest.Password, cancellationToken)) throw new UserAuthenticationException();

        var (response, _) = await sessions.CreateAsync(user, cancellationToken);
        await refreshTokens.SaveChangesAsync(cancellationToken);
        return response;
    }
}
