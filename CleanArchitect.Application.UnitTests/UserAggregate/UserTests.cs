using CleanArchitect.Domain.UserAggregate;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.UserAggregate;

public class UserTests
{
    [Fact]
    public void UpdateDisplayName_ShouldChangeDisplayName()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), Guid.NewGuid(), "Old Name");

        // Act
        user.UpdateDisplayName("New Name");

        // Assert
        user.DisplayName.ShouldBe("New Name");
    }

    [Fact]
    public void UpdateAddress_ShouldReplaceAddressesInPlace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(userId, userId);
        var originalAddresses = user.Addresses;
        user.UpdateAddress([new UserAddress(Guid.NewGuid(), userId, userId, "Australia", "Melbourne", "1 Main St", "0400000000")]);

        // Act
        user.UpdateAddress([new UserAddress(Guid.NewGuid(), userId, userId, "New Zealand", "Auckland", "2 Other St", "0400000001")]);

        // Assert
        user.Addresses.ShouldBeSameAs(originalAddresses);
        user.Addresses.ShouldHaveSingleItem();
        user.Addresses[0].City.ShouldBe("Auckland");
    }
}
