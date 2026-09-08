using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class LoginCommandHandlerTests
{
    private static readonly TokenSubject User = new(Guid.NewGuid(), "user@example.com", "user", "User");
    private readonly Mock<IRefreshTokenCookie> cookie = new();
    private readonly Mock<IRefreshTokenRepository> refreshTokens = new();
    private readonly Mock<ITokenService> tokens = new();
    private readonly Mock<IUserIdentityService> users = new();

    public LoginCommandHandlerTests()
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
    public async Task Handler_LoginWithInvalidCredentials_ShouldNotIssueTokens()
    {
        // Arrange
        users.Setup(u => u.FindByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(User);
        users.Setup(u => u.CheckPasswordAsync(User.Id, "wrong", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new LoginCommandHandler(users.Object, Sessions, refreshTokens.Object);

        // Act
        Func<Task> act = () => handler.Handle(
            new LoginCommand(new LoginRequest { Email = "user@example.com", Password = "wrong" }), default);

        // Assert
        await Should.ThrowAsync<UserAuthenticationException>(act);

        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
