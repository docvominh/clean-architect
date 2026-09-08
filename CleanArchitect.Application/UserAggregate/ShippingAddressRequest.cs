using System.ComponentModel.DataAnnotations;

namespace CleanArchitect.Application.UserAggregate;

public sealed record ShippingAddressRequest
{
    [Required] public required string Country { get; init; }

    public string? State { get; init; }

    [Required] public required string City { get; init; }

    [Required] public required string Street { get; init; }

    [Required] public required string ContactPhoneNumber { get; init; }
}
