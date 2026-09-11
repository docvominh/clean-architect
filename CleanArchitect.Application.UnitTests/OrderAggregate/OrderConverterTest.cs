using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Domain.OrderAggregate;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class OrderConverterTest
{
    [Fact]
    public void ToOrderDto_ShouldMapEveryPropertyAndProductInOrder()
    {
        // Arrange
        var createdAt = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
        var order = new Order(
            Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne",
            "1 Main St", "0400000000", "VIC", OrderStatus.Shipped) { CreatedAt = createdAt };
        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();
        order.AddProduct(firstProductId, 2, 12.50m);
        order.AddProduct(secondProductId, 3, 7.25m);
        var productNames = new Dictionary<Guid, string>
        {
            [secondProductId] = "Gadget",
            [firstProductId] = "Widget"
        };

        // Act
        var result = OrderConverter.ToOrderDto(order, productNames);

        // Assert
        result.Id.ShouldBe(order.Id);
        result.Status.ShouldBe(OrderStatus.Shipped);
        result.TotalAmount.ShouldBe(46.75m);
        result.CreatedAt.ShouldBe(createdAt);
        result.Country.ShouldBe("Australia");
        result.State.ShouldBe("VIC");
        result.City.ShouldBe("Melbourne");
        result.Street.ShouldBe("1 Main St");
        result.ContactPhoneNumber.ShouldBe("0400000000");
        result.Products.ShouldBe(
        [
            new OrderProductDto(firstProductId, "Widget", 2, 12.50m),
            new OrderProductDto(secondProductId, "Gadget", 3, 7.25m)
        ]);
    }

    [Fact]
    public void ToOrderDto_WithMissingProductName_ShouldUseUnknownProduct()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne",
            "1 Main St", "0400000000");
        var productId = Guid.NewGuid();
        order.AddProduct(productId, 2, 5m);
        var productNames = new Dictionary<Guid, string> { [Guid.NewGuid()] = "Unrelated product" };

        // Act
        var result = OrderConverter.ToOrderDto(order, productNames);

        // Assert
        result.Products.ShouldBe([new OrderProductDto(productId, "Unknown product", 2, 5m)]);
    }

    [Fact]
    public void ToOrderDto_WithoutStateOrProducts_ShouldPreserveNullAndEmptyList()
    {
        // Arrange
        var order = new Order(
            Guid.NewGuid(), Guid.NewGuid(), "New Zealand", "Auckland",
            "2 Other St", "0400000001", totalAmount: 15m);
        var productNames = new Dictionary<Guid, string>();

        // Act
        var result = OrderConverter.ToOrderDto(order, productNames);

        // Assert
        result.State.ShouldBeNull();
        result.Products.ShouldBeEmpty();
        result.TotalAmount.ShouldBe(15m);
    }

    [Fact]
    public void ToOrdersDto_ShouldMapEveryOrderInOrder()
    {
        // Arrange
        var first = new Order(
            Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne",
            "1 Main St", "0400000000", "VIC", OrderStatus.Confirmed);
        var second = new Order(
            Guid.NewGuid(), Guid.NewGuid(), "New Zealand", "Auckland",
            "2 Other St", "0400000001");
        var productId = Guid.NewGuid();
        first.AddProduct(productId, 2, 5m);
        second.AddProduct(productId, 3, 5m);
        var productNames = new Dictionary<Guid, string> { [productId] = "Widget" };

        // Act
        var result = OrderConverter.ToOrdersDto([first, second], productNames);

        // Assert
        result.Orders.Count.ShouldBe(2);
        result.Orders.Select(order => order.Id).ShouldBe([first.Id, second.Id]);
        result.Orders.Select(order => order.TotalAmount).ShouldBe([10m, 15m]);
        result.Orders[0].Products.ShouldBe([new OrderProductDto(productId, "Widget", 2, 5m)]);
        result.Orders[1].Products.ShouldBe([new OrderProductDto(productId, "Widget", 3, 5m)]);
    }

    [Fact]
    public void ToOrdersDto_WithEmptyCollection_ShouldReturnEmptyList()
    {
        // Arrange
        Order[] orders = [];
        var productNames = new Dictionary<Guid, string>();

        // Act
        var result = OrderConverter.ToOrdersDto(orders, productNames);

        // Assert
        result.Orders.ShouldBeEmpty();
    }
}
