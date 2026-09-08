using System.Security.Claims;

using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public Task<AuthResponse> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        return sender.Send(new RegisterCommand(request), cancellationToken);
    }

    [HttpPost("login")]
    public Task<AuthResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return sender.Send(new LoginCommand(request), cancellationToken);
    }

    [HttpPost("refresh")]
    public Task<AuthResponse> Refresh(CancellationToken cancellationToken)
    {
        return sender.Send(new RefreshTokenCommand(), cancellationToken);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await sender.Send(new RevokeTokenCommand(userId), cancellationToken);
        return NoContent();
    }
}
