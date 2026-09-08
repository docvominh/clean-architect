using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class LoginCommand(LoginRequest request) : IRequest<AuthResponse>
{
    public LoginRequest LoginRequest { get; set; } = request;
}
