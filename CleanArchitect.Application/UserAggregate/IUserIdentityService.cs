namespace CleanArchitect.Application.UserAggregate;

public interface IUserIdentityService
{
    Task<TokenSubject> CreateAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken);
    Task<TokenSubject?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<TokenSubject?> FindByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
    Task<IList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);
}

