using CleanArchitect.Application.UserAggregate;
using CleanArchitect.Application.UserAggregate.AspnetIdentity;
using CleanArchitect.Application.UserAggregate.Command;
using CleanArchitect.Domain.UserAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IUserIdentityService> identity = new();
    private readonly Mock<IUserRepository> users = new();

    private static UpdateProfileRequest Request(params ShippingAddressRequest[] addresses)
    {
        return new UpdateProfileRequest { DisplayName = "New Name", Addresses = addresses.ToList() };
    }

    private static ShippingAddressRequest Address(string city, bool isDefault = false)
    {
        return new ShippingAddressRequest
            { Country = "Australia", City = city, Street = "1 Main St", ContactPhoneNumber = "0400000000", IsDefault = isDefault };
    }

    [Fact]
    public async Task Handler_UpdateProfile_ShouldChangeDisplayNameAndSave()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new User(userId, userId, "Old Name");
        users.Setup(u => u.FindAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);
        identity.Setup(i => i.FindByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenSubject(userId, "user@example.com", "user@example.com", "New Name"));
        var handler = new UpdateProfileCommandHandler(users.Object, identity.Object);

        // Act
        var result = await handler.Handle(new UpdateProfileCommand(userId, Request(Address("Melbourne", true))), default);

        // Assert
        result.DisplayName.ShouldBe("New Name");
        profile.DisplayName.ShouldBe("New Name");
        users.Verify(u => u.AddAddresses(It.Is<IEnumerable<UserAddress>>(a => a.Count() == 1)), Times.Once);
        users.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_UpdateProfileWithNoDefaultMarked_ShouldFallBackToFirstAddress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new User(userId, userId);
        users.Setup(u => u.FindAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);
        var handler = new UpdateProfileCommandHandler(users.Object, identity.Object);

        // Act
        await handler.Handle(new UpdateProfileCommand(userId, Request(Address("Melbourne"), Address("Sydney"))), default);

        // Assert
        profile.Addresses.Count(a => a.IsDefault).ShouldBe(1);
        profile.Addresses[0].IsDefault.ShouldBeTrue();
    }

    [Fact]
    public async Task Handler_UpdateProfileWithMultipleDefaultsMarked_ShouldKeepOnlyOneDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profile = new User(userId, userId);
        users.Setup(u => u.FindAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);
        var handler = new UpdateProfileCommandHandler(users.Object, identity.Object);

        // Act
        await handler.Handle(new UpdateProfileCommand(userId, Request(Address("Melbourne", true), Address("Sydney", true))), default);

        // Assert
        profile.Addresses.Count(a => a.IsDefault).ShouldBe(1);
        profile.Addresses[0].IsDefault.ShouldBeTrue();
        profile.Addresses[1].IsDefault.ShouldBeFalse();
    }

    [Fact]
    public async Task Handler_UpdateMissingProfile_ShouldThrowNotFoundException()
    {
        // Arrange
        users.Setup(u => u.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var handler = new UpdateProfileCommandHandler(users.Object, identity.Object);

        // Act
        var act = () => handler.Handle(new UpdateProfileCommand(Guid.NewGuid(), Request()), default);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        users.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
