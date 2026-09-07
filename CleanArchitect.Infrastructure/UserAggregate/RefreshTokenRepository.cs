using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken) =>
        dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

    public void Add(RefreshToken token) => dbContext.RefreshTokens.Add(token);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}

