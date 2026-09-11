using CleanArchitect.Domain.OrderAggregate;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class OrderValidationTests
{
    [Theory]
    [InlineData(0, null)]
    [InlineData(0, "")]
    [InlineData(0, " ")]
    [InlineData(1, null)]
    [InlineData(1, "")]
    [InlineData(1, " ")]
    [InlineData(2, null)]
    [InlineData(2, "")]
    [InlineData(2, " ")]
    [InlineData(3, null)]
    [InlineData(3, "")]
    [InlineData(3, " ")]
    public void Constructor_RejectsMissingShippingFields(int index, string? value)
    {
        // Arrange
        string?[] fields = ["Country", "City", "Street", "Phone"];
        string[] parameters = ["country", "city", "street", "contactPhoneNumber"];
        fields[index] = value;

        // Act
        Action act = () => _ = new Order(Guid.NewGuid(), Guid.NewGuid(), fields[0]!, fields[1]!, fields[2]!, fields[3]!);

        // Assert
        Should.Throw<ArgumentException>(act).ParamName.ShouldBe(parameters[index]);
    }

    [Theory]
    [InlineData(-1, 0, "status")]
    [InlineData(999, 0, "status")]
    [InlineData(0, -1, "totalAmount")]
    public void Constructor_RejectsInvalidStatusOrTotal(int status, int total, string parameter)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => _ = new Order(id, id, "Country", "City", "Street", "Phone", status: (OrderStatus)status, totalAmount: total);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(act).ParamName.ShouldBe(parameter);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Constructor_AcceptsDefinedStatusesAndZeroTotal(int status)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var order = new Order(id, id, "Country", "City", "Street", "Phone", status: (OrderStatus)status);

        // Assert
        order.Status.ShouldBe((OrderStatus)status);
        order.TotalAmount.ShouldBe(0);
        order.OrderProducts.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(0, 10, "quantity")]
    [InlineData(-1, 10, "quantity")]
    [InlineData(1, -1, "unitPrice")]
    public void ProductMutations_WhenInvalid_PreserveOrder(int quantity, int unitPrice, string parameter)
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), "Country", "City", "Street", "Phone");
        var productId = Guid.NewGuid();
        order.AddProduct(productId, 2, 10);
        var original = order.OrderProducts[0];

        // Act
        var add = () => order.AddProduct(Guid.NewGuid(), quantity, unitPrice);
        var update = () => order.UpdateProduct(productId, quantity, unitPrice);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(add).ParamName.ShouldBe(parameter);
        Should.Throw<ArgumentOutOfRangeException>(update).ParamName.ShouldBe(parameter);
        order.OrderProducts.ShouldHaveSingleItem().ShouldBeSameAs(original);
        order.TotalAmount.ShouldBe(20);
    }

    [Fact]
    public void UpdateStatus_RejectsUndefinedValueWithoutChangingStatus()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), "Country", "City", "Street", "Phone");

        // Act
        var act = () => order.UpdateStatus((OrderStatus)999);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(act).ParamName.ShouldBe("status");
        order.Status.ShouldBe(OrderStatus.Pending);
    }
}
