using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Domain.UserAggregate;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.UserProfiles
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task Add(User user)
    {
        await dbContext.UserProfiles.AddAsync(user);
    }

    public void AddAddresses(IEnumerable<UserAddress> addresses)
    {
        dbContext.UserAddresses.AddRange(addresses);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
