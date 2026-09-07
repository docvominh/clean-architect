using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;
using Xunit;

namespace CleanArchitect.Application.Tests;

public class UserCommandTests
{
    private readonly IdentityStub users = new();
    private readonly TokenRepository tokens = new();
    private readonly CookieStub cookie = new();
    private AuthSessionService Sessions => new(users, new TokenStub(), tokens, cookie);

    [Fact]
    public async Task InvalidLoginDoesNotIssueTokens()
    {
        users.PasswordValid = false;
        var handler = new LoginCommandHandler(users, Sessions, tokens);
        await Assert.ThrowsAsync<UserAuthenticationException>(() => handler.Handle(
            new LoginCommand(new LoginRequest { Email = "user@example.com", Password = "wrong" }), default));
        Assert.Empty(tokens.Added);
        Assert.Equal(0, tokens.Saves);
    }

    [Fact]
    public async Task RegistrationAssignsUserRoleAndPersistsSession()
    {
        var handler = new RegisterCommandHandler(users, Sessions, tokens);
        var result = await handler.Handle(new RegisterCommand(new RegisterRequest
            { Email = "user@example.com", Password = "Password123!" }), default);
        Assert.Equal("User", users.AssignedRole);
        Assert.Equal(tokens.Added.Single().Token, cookie.WrittenToken);
        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal(1, tokens.Saves);
    }

    [Fact]
    public async Task RoleAssignmentFailureDoesNotIssueTokens()
    {
        users.FailRoleAssignment = true;
        var handler = new RegisterCommandHandler(users, Sessions, tokens);
        await Assert.ThrowsAsync<UserValidationException>(() => handler.Handle(
            new RegisterCommand(new RegisterRequest { Email = "user@example.com", Password = "Password123!" }), default));
        Assert.Empty(tokens.Added);
    }

    [Fact]
    public async Task RefreshRotatesAndSavesBothTokensTogether()
    {
        var old = tokens.Existing = new RefreshToken { UserId = users.User.Id, Token = "old", ExpiresAt = DateTimeOffset.UtcNow.AddDays(1) };
        cookie.ValueToRead = "old";
        await new RefreshTokenCommandHandler(users, tokens, Sessions, cookie)
            .Handle(new RefreshTokenCommand(), default);
        Assert.NotNull(old.RevokedAt);
        Assert.Equal(cookie.WrittenToken, old.ReplacedByToken);
        Assert.True(tokens.OldTokenWasRevokedAtSave);
        Assert.Single(tokens.Added);
        Assert.Equal(1, tokens.Saves);
    }

    [Fact]
    public async Task RevokedRefreshTokenCannotCreateSession()
    {
        tokens.Existing = new RefreshToken { UserId = users.User.Id, Token = "old",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1), RevokedAt = DateTimeOffset.UtcNow };
        cookie.ValueToRead = "old";
        await Assert.ThrowsAsync<UserAuthenticationException>(() =>
            new RefreshTokenCommandHandler(users, tokens, Sessions, cookie).Handle(new RefreshTokenCommand(), default));
        Assert.Empty(tokens.Added);
        Assert.Equal(0, tokens.Saves);
        Assert.True(cookie.Cleared);
    }

    [Fact]
    public async Task RevokeCannotRevokeAnotherUsersToken()
    {
        tokens.Existing = new RefreshToken { UserId = users.User.Id, Token = "old", ExpiresAt = DateTimeOffset.UtcNow.AddDays(1) };
        cookie.ValueToRead = "old";
        await new RevokeTokenCommandHandler(tokens, cookie).Handle(new RevokeTokenCommand(Guid.NewGuid()), default);
        Assert.Null(tokens.Existing.RevokedAt);
        Assert.Equal(0, tokens.Saves);
        Assert.True(cookie.Cleared);
    }

    private sealed class TokenRepository : IRefreshTokenRepository
    {
        public RefreshToken? Existing;
        public List<RefreshToken> Added { get; } = [];
        public int Saves;
        public bool OldTokenWasRevokedAtSave;
        public Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken) =>
            Task.FromResult(Existing?.Token == token ? Existing : null);
        public void Add(RefreshToken token) => Added.Add(token);
        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            Saves++;
            OldTokenWasRevokedAtSave = Existing?.RevokedAt is not null;
            return Task.CompletedTask;
        }
    }

    private sealed class TokenStub : ITokenService
    {
        public (string AccessToken, DateTimeOffset ExpiresAt) GenerateAccessToken(TokenSubject user, IList<string> roles) =>
            ("access-token", DateTimeOffset.UtcNow.AddMinutes(15));
        public RefreshToken GenerateRefreshToken(Guid userId) => new()
            { UserId = userId, Token = "new-token", ExpiresAt = DateTimeOffset.UtcNow.AddDays(14) };
    }

    private sealed class CookieStub : IRefreshTokenCookie
    {
        public string? ValueToRead;
        public string? WrittenToken;
        public bool Cleared;
        public string? Read() => ValueToRead;
        public void Write(string token, DateTimeOffset expiresAt) => WrittenToken = token;
        public void Clear() => Cleared = true;
    }

    private sealed class IdentityStub : IUserIdentityService
    {
        public TokenSubject User { get; } = new(Guid.NewGuid(), "user@example.com", "user", "User");
        public bool PasswordValid = true;
        public bool FailRoleAssignment;
        public string? AssignedRole;
        public Task<TokenSubject> CreateAsync(RegisterRequest request, CancellationToken cancellationToken) => Task.FromResult(User);
        public Task AddToRoleAsync(Guid id, string role, CancellationToken cancellationToken)
        {
            if (FailRoleAssignment) throw new UserValidationException(new Dictionary<string, string[]> { ["Role"] = ["Role assignment failed"] });
            AssignedRole = role;
            return Task.CompletedTask;
        }
        public Task<TokenSubject?> FindByEmailAsync(string email, CancellationToken cancellationToken) => Task.FromResult<TokenSubject?>(User);
        public Task<TokenSubject?> FindByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<TokenSubject?>(User);
        public Task<bool> CheckPasswordAsync(Guid id, string password, CancellationToken cancellationToken) => Task.FromResult(PasswordValid);
        public Task<IList<string>> GetRolesAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<IList<string>>(["User"]);
    }
}
