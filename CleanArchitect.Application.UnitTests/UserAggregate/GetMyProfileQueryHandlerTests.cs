using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Query;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class GetMyProfileQueryHandlerTests
{
    private readonly Mock<IUserIdentityService> identity = new();
    private readonly Mock<IUserRepository> users = new();

    [Fact]
    public async Task Handler_GetMyProfile_ShouldMapEmailDisplayNameAndAddresses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new User(userId, userId, "Test User");
        profile.UpdateAddress([new UserAddress(Guid.NewGuid(), userId, userId, "Australia", "Melbourne", "1 Main St", "0400000000", "VIC", true)]);
        users.Setup(u => u.FindAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);
        identity.Setup(i => i.FindByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenSubject(userId, "user@example.com", "user@example.com", "Test User"));
        var handler = new GetMyProfileQueryHandler(users.Object, identity.Object);

        // Act
        var result = await handler.Handle(new GetMyProfileQuery(userId), default);

        // Assert
        result.Email.ShouldBe("user@example.com");
        result.DisplayName.ShouldBe("Test User");
        result.Addresses.ShouldHaveSingleItem();
        result.Addresses[0].City.ShouldBe("Melbourne");
        result.Addresses[0].IsDefault.ShouldBeTrue();
    }

    [Fact]
    public async Task Handler_GetMyProfileWhenMissing_ShouldThrowNotFoundException()
    {
        // Arrange
        users.Setup(u => u.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var handler = new GetMyProfileQueryHandler(users.Object, identity.Object);

        // Act
        var act = () => handler.Handle(new GetMyProfileQuery(Guid.NewGuid()), default);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
    }
}
