using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Query;
using CleanArchitect.Domain.UserAggregate;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public sealed class UpdateProfileCommandHandler(IUserRepository users, IUserIdentityService identity) : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    public async Task<ProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await users.FindAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User '{request.UserId}' was not found.");

        profile.UpdateDisplayName(request.Request.DisplayName);

        var addresses = request.Request.Addresses;
        var defaultIndex = addresses.FindIndex(address => address.IsDefault);
        if (defaultIndex < 0) defaultIndex = 0;

        var newAddresses = addresses.Select((address, i) => new UserAddress(
                Guid.NewGuid(),
                request.UserId,
                request.UserId,
                address.Country,
                address.City,
                address.Street,
                address.ContactPhoneNumber,
                address.State,
                i == defaultIndex))
            .ToList();

        profile.UpdateAddress(newAddresses);
        users.AddAddresses(newAddresses);

        await users.SaveChangesAsync(cancellationToken);

        var subject = await identity.FindByIdAsync(request.UserId, cancellationToken);

        return ProfileDto.From(subject?.Email, profile);
    }
}
