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
        var user = new User
        {
            Id = subject.Id,
            DisplayName = request.RegisterRequest.DisplayName,
            CreateBy = subject.Id,
            UpdateBy = subject.Id,
            Addresses = addresses.Select((address, i) => new UserAddress
            {
                UserId = subject.Id,
                IsDefault = i == 0,
                Country = address.Country,
                State = address.State,
                City = address.City,
                Street = address.Street,
                ContactPhoneNumber = address.ContactPhoneNumber,
            }).ToList(),
        };
        await userRepository.Add(user);

        var (response, _) = await sessions.CreateAsync(subject with { DisplayName = request.RegisterRequest.DisplayName }, cancellationToken);
        await refreshTokens.SaveChangesAsync(cancellationToken);

        return response;
    }
}
