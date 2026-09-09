using CleanArchitect.Domain.UserAggregate;

namespace CleanArchitect.Application.UserAggregate.AspnetIdentity;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken);

    void Add(RefreshToken token);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
