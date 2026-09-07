using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests;

public class UserCommandTests
{
    private static readonly TokenSubject User = new(Guid.NewGuid(), "user@example.com", "user", "User");
    private readonly Mock<IRefreshTokenCookie> cookie = new();
    private readonly Mock<IRefreshTokenRepository> refreshTokens = new();
    private readonly Mock<ITokenService> tokens = new();

    private readonly Mock<IUserIdentityService> users = new();

    public UserCommandTests()
    {
        tokens.Setup(t => t.GenerateAccessToken(It.IsAny<TokenSubject>(), It.IsAny<IList<string>>()))
            .Returns(("access-token", DateTimeOffset.UtcNow.AddMinutes(15)));
        tokens.Setup(t => t.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns((Guid userId) => new RefreshToken { UserId = userId, Token = "new-token", ExpiresAt = DateTimeOffset.UtcNow.AddDays(14) });
        users.Setup(u => u.GetRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "User" });
    }

    private AuthSessionService Sessions => new(users.Object, tokens.Object, refreshTokens.Object, cookie.Object);

    [Fact]
    public async Task InvalidLoginDoesNotIssueTokens()
    {
        users.Setup(u => u.FindByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(User);
        users.Setup(u => u.CheckPasswordAsync(User.Id, "wrong", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new LoginCommandHandler(users.Object, Sessions, refreshTokens.Object);

        await Should.ThrowAsync<UserAuthenticationException>(() => handler.Handle(
            new LoginCommand(new LoginRequest { Email = "user@example.com", Password = "wrong" }), default));

        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegistrationAssignsUserRoleAndPersistsSession()
    {
        users.Setup(u => u.CreateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(User);

        var handler = new RegisterCommandHandler(users.Object, Sessions, refreshTokens.Object);
        var result = await handler.Handle(
            new RegisterCommand(
                new RegisterRequest
                    { Email = "user@example.com", Password = "Password123!" }), default);

        users.Verify(u => u.AddToRoleAsync(User.Id, "User", It.IsAny<CancellationToken>()), Times.Once);
        refreshTokens.Verify(r => r.Add(It.Is<RefreshToken>(t => t.Token == "new-token")), Times.Once);
        cookie.Verify(c => c.Write("new-token", It.IsAny<DateTimeOffset>()), Times.Once);
        result.AccessToken.ShouldBe("access-token");
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RoleAssignmentFailureDoesNotIssueTokens()
    {
        users.Setup(u => u.CreateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(User);
        users.Setup(u => u.AddToRoleAsync(User.Id, "User", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserValidationException(new Dictionary<string, string[]> { ["Role"] = ["Role assignment failed"] }));

        var handler = new RegisterCommandHandler(users.Object, Sessions, refreshTokens.Object);

        await Should.ThrowAsync<UserValidationException>(() => handler.Handle(
            new RegisterCommand(new RegisterRequest { Email = "user@example.com", Password = "Password123!" }), default));

        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshRotatesAndSavesBothTokensTogether()
    {
        var old = new RefreshToken { UserId = User.Id, Token = "old", ExpiresAt = DateTimeOffset.UtcNow.AddDays(1) };
        cookie.Setup(c => c.Read()).Returns("old");
        refreshTokens.Setup(r => r.FindAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(old);
        users.Setup(u => u.FindByIdAsync(User.Id, It.IsAny<CancellationToken>())).ReturnsAsync(User);

        var handler = new RefreshTokenCommandHandler(users.Object, refreshTokens.Object, Sessions, cookie.Object);
        await handler.Handle(new RefreshTokenCommand(), default);

        old.RevokedAt.ShouldNotBeNull();
        old.ReplacedByToken.ShouldBe("new-token");
        refreshTokens.Verify(r => r.Add(It.Is<RefreshToken>(t => t.Token == "new-token")), Times.Once);
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RevokedRefreshTokenCannotCreateSession()
    {
        var old = new RefreshToken
        {
            UserId = User.Id, Token = "old", ExpiresAt = DateTimeOffset.UtcNow.AddDays(1), RevokedAt = DateTimeOffset.UtcNow
        };
        cookie.Setup(c => c.Read()).Returns("old");
        refreshTokens.Setup(r => r.FindAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(old);

        var handler = new RefreshTokenCommandHandler(users.Object, refreshTokens.Object, Sessions, cookie.Object);

        await Should.ThrowAsync<UserAuthenticationException>(() => handler.Handle(new RefreshTokenCommand(), default));

        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cookie.Verify(c => c.Clear(), Times.Once);
    }

    [Fact]
    public async Task RevokeCannotRevokeAnotherUsersToken()
    {
        var old = new RefreshToken { UserId = User.Id, Token = "old", ExpiresAt = DateTimeOffset.UtcNow.AddDays(1) };
        cookie.Setup(c => c.Read()).Returns("old");
        refreshTokens.Setup(r => r.FindAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(old);

        var handler = new RevokeTokenCommandHandler(refreshTokens.Object, cookie.Object);
        await handler.Handle(new RevokeTokenCommand(Guid.NewGuid()), default);

        old.RevokedAt.ShouldBeNull();
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cookie.Verify(c => c.Clear(), Times.Once);
    }
}
