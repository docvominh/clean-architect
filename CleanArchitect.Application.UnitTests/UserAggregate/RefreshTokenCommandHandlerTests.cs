using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class RefreshTokenCommandHandlerTests
{
    private static readonly TokenSubject User = new(Guid.NewGuid(), "user@example.com", "user", "User");
    private readonly Mock<IRefreshTokenCookie> cookie = new();
    private readonly Mock<IRefreshTokenRepository> refreshTokens = new();
    private readonly Mock<ITokenService> tokens = new();
    private readonly Mock<IUserIdentityService> users = new();

    public RefreshTokenCommandHandlerTests()
    {
        tokens.Setup(t => t.GenerateAccessToken(It.IsAny<TokenSubject>(), It.IsAny<IList<string>>()))
            .Returns(("access-token", DateTimeOffset.UtcNow.AddMinutes(15)));
        tokens.Setup(t => t.GenerateRefreshToken(It.IsAny<Guid>()))
            .Returns((Guid userId) => new RefreshToken(Guid.NewGuid(), userId, userId, "new-token", DateTimeOffset.UtcNow.AddDays(14)));
        users.Setup(u => u.GetRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "User" });
    }

    private AuthSessionService Sessions => new(users.Object, tokens.Object, refreshTokens.Object, cookie.Object);

    [Fact]
    public async Task Handler_RefreshToken_ShouldRotateAndSaveBothTokensTogether()
    {
        var old = new RefreshToken(Guid.NewGuid(), User.Id, User.Id, "old", DateTimeOffset.UtcNow.AddDays(1));
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
    public async Task Handler_RefreshWithRevokedToken_ShouldNotCreateSession()
    {
        var old = new RefreshToken(Guid.NewGuid(), User.Id, User.Id, "old", DateTimeOffset.UtcNow.AddDays(1)) { RevokedAt = DateTimeOffset.UtcNow };
        cookie.Setup(c => c.Read()).Returns("old");
        refreshTokens.Setup(r => r.FindAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(old);

        var handler = new RefreshTokenCommandHandler(users.Object, refreshTokens.Object, Sessions, cookie.Object);

        await Should.ThrowAsync<UserAuthenticationException>(() => handler.Handle(new RefreshTokenCommand(), default));

        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cookie.Verify(c => c.Clear(), Times.Once);
    }
}
