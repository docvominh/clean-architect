using CleanArchitect.Application.UserAggregate.Query;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Command;

public sealed record UpdateProfileCommand(Guid UserId, UpdateProfileRequest Request) : IRequest<ProfileDto>;
