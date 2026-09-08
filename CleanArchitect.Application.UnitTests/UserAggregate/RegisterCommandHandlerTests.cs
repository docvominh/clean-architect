using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class RegisterCommandHandlerTests
{
    private static readonly TokenSubject User = new(Guid.NewGuid(), "user@example.com", "user", "User");
    private readonly Mock<IRefreshTokenCookie> cookie = new();
    private readonly Mock<IRefreshTokenRepository> refreshTokens = new();
    private readonly Mock<ITokenService> tokens = new();
    private readonly Mock<IUserRepository> userRepository = new();
    private readonly Mock<IUserIdentityService> users = new();

    public RegisterCommandHandlerTests()
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
    public async Task Handler_Register_ShouldAssignUserRoleAndPersistSession()
    {
        users.Setup(u => u.CreateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(User);

        var handler = new RegisterCommandHandler(users.Object, userRepository.Object, Sessions, refreshTokens.Object);
        var result = await handler.Handle(
            new RegisterCommand(
                new RegisterRequest
                    { Email = "user@example.com", Password = "Password123!", DisplayName = "Test User" }), default);

        users.Verify(u => u.AddToRoleAsync(User.Id, "User", It.IsAny<CancellationToken>()), Times.Once);
        userRepository.Verify(r => r.Add(It.Is<Domain.UserAggregate.User>(u =>
            u.Id == User.Id && u.DisplayName == "Test User" && u.Addresses.Count == 0
            && u.CreateBy == User.Id && u.UpdateBy == User.Id)), Times.Once);
        tokens.Verify(t => t.GenerateAccessToken(It.Is<TokenSubject>(s => s.DisplayName == "Test User"), It.IsAny<IList<string>>()), Times.Once);
        refreshTokens.Verify(r => r.Add(It.Is<RefreshToken>(t => t.Token == "new-token")), Times.Once);
        cookie.Verify(c => c.Write("new-token", It.IsAny<DateTimeOffset>()), Times.Once);
        result.AccessToken.ShouldBe("access-token");
        refreshTokens.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_RegisterWithAddresses_ShouldMarkOnlyFirstAddressAsDefault()
    {
        users.Setup(u => u.CreateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(User);

        var handler = new RegisterCommandHandler(users.Object, userRepository.Object, Sessions, refreshTokens.Object);
        await handler.Handle(
            new RegisterCommand(
                new RegisterRequest
                {
                    Email = "user@example.com",
                    Password = "Password123!",
                    Addresses =
                    [
                        new ShippingAddressRequest { Country = "US", City = "Springfield", Street = "1 Main St", ContactPhoneNumber = "555-0100" },
                        new ShippingAddressRequest { Country = "US", City = "Shelbyville", Street = "2 Elm St", ContactPhoneNumber = "555-0200" },
                    ],
                }), default);

        userRepository.Verify(r => r.Add(It.Is<Domain.UserAggregate.User>(u =>
            u.Addresses.Count == 2
            && u.Addresses[0].UserId == User.Id && u.Addresses[0].City == "Springfield" && u.Addresses[0].IsDefault
            && u.Addresses[0].CreateBy == User.Id && u.Addresses[0].UpdateBy == User.Id
            && u.Addresses[1].UserId == User.Id && u.Addresses[1].City == "Shelbyville" && !u.Addresses[1].IsDefault
            && u.Addresses[1].CreateBy == User.Id && u.Addresses[1].UpdateBy == User.Id)), Times.Once);
    }

    [Fact]
    public async Task Handler_RegisterWithRoleAssignmentFailure_ShouldNotIssueTokens()
    {
        users.Setup(u => u.CreateAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(User);
        users.Setup(u => u.AddToRoleAsync(User.Id, "User", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UserValidationException(new Dictionary<string, string[]> { ["Role"] = ["Role assignment failed"] }));

        var handler = new RegisterCommandHandler(users.Object, userRepository.Object, Sessions, refreshTokens.Object);

        await Should.ThrowAsync<UserValidationException>(() => handler.Handle(
            new RegisterCommand(new RegisterRequest { Email = "user@example.com", Password = "Password123!" }), default));

        userRepository.Verify(r => r.Add(It.IsAny<Domain.UserAggregate.User>()), Times.Never);
        refreshTokens.Verify(r => r.Add(It.IsAny<RefreshToken>()), Times.Never);
    }
}
