using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate.Query;

public sealed record ProfileDto(string? Email, string? DisplayName, IReadOnlyList<ProfileAddressDto> Addresses)
{
    public static ProfileDto From(string? email, User user)
    {
        return new ProfileDto(
            email,
            user.DisplayName,
            user.Addresses
                .Select(address => new ProfileAddressDto(address.Id, address.Country, address.State, address.City, address.Street, address.ContactPhoneNumber, address.IsDefault))
                .ToList());
    }
}

public sealed record ProfileAddressDto(Guid Id, string Country, string? State, string City, string Street, string ContactPhoneNumber, bool IsDefault);
