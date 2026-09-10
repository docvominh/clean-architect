namespace CleanArchitect.Application.UserAggregate;

public sealed record UpdateProfileRequest
{
    public string? DisplayName { get; init; }

    public required List<ShippingAddressRequest> Addresses { get; init; }
}
