using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.UserProfiles.FindAsync([id], cancellationToken);
    }

    public async Task Add(User user)
    {
        await dbContext.UserProfiles.AddAsync(user);
    }
}
