using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class RevokeTokenCommandHandlerTests
{
    private static readonly TokenSubject User = new(Guid.NewGuid(), "user@example.com", "user", "User");
    private readonly Mock<IRefreshTokenCookie> cookie = new();
    private readonly Mock<IRefreshTokenRepository> refreshTokens = new();

    [Fact]
    public async Task Handler_RevokeAnotherUsersToken_ShouldNotRevokeToken()
    {
        // Arrange
        var old = new RefreshToken(Guid.NewGuid(), User.Id, User.Id, "old", DateTimeOffset.UtcNow.AddDays(1));
        cookie.Setup(c => c.Read()).Returns("old");
        refreshTokens.Setup(r => r.FindAsync("old", It.IsAny<CancellationToken>())).ReturnsAsync(old);

        var handler = new RevokeTokenCommandHandler(refreshTokens.Object, cookie.Object);

        // Act
        await handler.Handle(new RevokeTokenCommand(Guid.NewGuid()), default);

        // Assert
        old.RevokedAt.ShouldBeNull();
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cookie.Verify(c => c.Clear(), Times.Once);
    }
}
