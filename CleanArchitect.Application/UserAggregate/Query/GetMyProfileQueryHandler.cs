using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using MediatR;

namespace CleanArchitect.Application.UserAggregate.Query;

public sealed class GetMyProfileQueryHandler(IUserRepository users, IUserIdentityService identity) : IRequestHandler<GetMyProfileQuery, ProfileDto>
{
    public async Task<ProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await users.FindAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User '{request.UserId}' was not found.");

        var subject = await identity.FindByIdAsync(request.UserId, cancellationToken);

        return ProfileDto.From(subject?.Email, profile);
    }
}
