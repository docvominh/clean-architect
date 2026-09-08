using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public class RevokeTokenCommand(Guid userId) : IRequest
{
    public Guid UserId { get; } = userId;
}
