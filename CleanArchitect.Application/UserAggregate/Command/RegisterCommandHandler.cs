using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Domain;
using CleanArchitect.Domain.UserAggregate;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RegisterCommandHandler(
    IUserIdentityService users,
    IUserRepository userRepository,
    AuthSessionService sessions,
    IRefreshTokenRepository refreshTokens) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var subject = await users.CreateAsync(request.RegisterRequest, cancellationToken);
        await users.AddToRoleAsync(subject.Id, "User", cancellationToken);

        var addresses = request.RegisterRequest.Addresses ?? [];
        var user = new User(subject.Id, subject.Id, request.RegisterRequest.DisplayName);
        user.UpdateAddress(addresses.Select((address, i) => new UserAddress(
            Guid.NewGuid(),
            subject.Id,
            subject.Id,
            address.Country,
            address.City,
            address.Street,
            address.ContactPhoneNumber,
            address.State,
            isDefault: i == 0)).ToList());
        await userRepository.Add(user);

        var (response, _) = await sessions.CreateAsync(subject with { DisplayName = request.RegisterRequest.DisplayName }, cancellationToken);
        await refreshTokens.SaveChangesAsync(cancellationToken);

        return response;
    }
}
