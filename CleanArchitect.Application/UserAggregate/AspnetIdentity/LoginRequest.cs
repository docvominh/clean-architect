using System.ComponentModel.DataAnnotations;

namespace CleanArchitect.Application.UserAggregate.AspnetIdentity;

public sealed record LoginRequest
{
    [Required] [EmailAddress] public required string Email { get; init; }

    [Required] public required string Password { get; init; }
}
