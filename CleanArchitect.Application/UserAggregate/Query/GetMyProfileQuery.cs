using MediatR;

namespace CleanArchitect.Application.UserAggregate.Query;

public sealed record GetMyProfileQuery(Guid UserId) : IRequest<ProfileDto>;
