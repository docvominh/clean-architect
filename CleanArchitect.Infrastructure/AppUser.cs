using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace CleanArchitect.Infrastructure;

public class AppUser : IdentityUser<Guid>
{
    [MaxLength(256)] public string? DisplayName { get; init; }
}
