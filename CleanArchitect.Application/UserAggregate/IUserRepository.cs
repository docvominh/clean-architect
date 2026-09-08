using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate;

public interface IUserRepository
{
    Task<User?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task Add(User user);
}
