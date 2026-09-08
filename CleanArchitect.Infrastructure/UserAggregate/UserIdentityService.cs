using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;

using Microsoft.AspNetCore.Identity;

namespace CleanArchitect.Infrastructure.UserAggregate;

public sealed class UserIdentityService(UserManager<AppUser> users, SignInManager<AppUser> signIn, IUserRepository userRepository) : IUserIdentityService
{
    public async Task<TokenSubject> CreateAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = new AppUser { UserName = request.Email, Email = request.Email };
        EnsureSuccess(await users.CreateAsync(user, request.Password));
        return new TokenSubject(user.Id, user.Email, user.UserName, null);
    }

    public async Task AddToRoleAsync(Guid userId, string role, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        EnsureSuccess(await users.AddToRoleAsync(user, role));
    }

    public async Task<TokenSubject?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByEmailAsync(email);
        return user is null ? null : await SubjectAsync(user, cancellationToken);
    }

    public async Task<TokenSubject?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await users.FindByIdAsync(userId.ToString());
        return user is null ? null : await SubjectAsync(user, cancellationToken);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(userId, cancellationToken);
        return (await signIn.CheckPasswordSignInAsync(user, password, true)).Succeeded;
    }

    public async Task<IList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await users.GetRolesAsync(await GetUserAsync(userId, cancellationToken));
    }

    private async Task<AppUser> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await users.FindByIdAsync(userId.ToString()) ?? throw new UserAuthenticationException();
    }

    private async Task<TokenSubject> SubjectAsync(AppUser user, CancellationToken cancellationToken)
    {
        var profile = await userRepository.FindAsync(user.Id, cancellationToken);
        return new TokenSubject(user.Id, user.Email, user.UserName, profile?.DisplayName);
    }

    private static void EnsureSuccess(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new UserValidationException(
                result.Errors.GroupBy(e => e.Code)
                    .ToDictionary(group => group.Key, group => group.Select(e => e.Description).ToArray()));
    }
}
