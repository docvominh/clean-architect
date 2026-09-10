using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate;

public interface IUserRepository
{
    Task<User?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task Add(User user);

    // Explicitly marks new address rows for insertion. EF Core can't reliably infer Added vs.
    // Modified for entities with client-generated (non-default) Guid keys that are discovered via
    // collection-navigation fixup on an already-persisted parent, so this must be explicit.
    void AddAddresses(IEnumerable<UserAddress> addresses);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
