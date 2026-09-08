namespace CleanArchitect.Domain.UserAggregate;

public class UserAddress : BaseEntity
{
    public required Guid UserId { get; set; }

    public required string Country { get; init; }

    public string? State { get; init; }

    public required string City { get; init; }

    public required string Street { get; init; }

    public required string ContactPhoneNumber { get; init; }


    public bool IsDefault { get; set; }
}
