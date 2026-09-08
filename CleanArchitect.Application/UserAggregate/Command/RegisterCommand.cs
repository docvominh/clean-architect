using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RegisterCommand : IRequest<AuthResponse>
{
    public RegisterCommand(RegisterRequest request)
    {
        RegisterRequest = request;
    }

    public RegisterRequest RegisterRequest { get; set; }
}
