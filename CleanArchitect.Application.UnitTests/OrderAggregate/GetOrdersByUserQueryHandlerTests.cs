using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Query;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.OrderAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class GetOrdersByUserQueryHandlerTests
{
    private readonly Mock<IOrderRepository> orders = new();
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_GetMyOrders_ShouldResolveProductNames()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var product = new Product(Guid.NewGuid(), userId, "Widget", "Acme", 10m);
        var order = new Order(Guid.NewGuid(), userId, "Australia", "Melbourne", "1 Main St", "0400000000");
        order.AddProduct(product.Id, 2, 10m);
        orders.Setup(o => o.GetByUserAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([order]);
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([product]);
        var handler = new GetOrdersByUserQueryHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new GetOrdersByUserQuery(userId), default);

        // Assert
        result.Orders.ShouldHaveSingleItem();
        result.Orders[0].Id.ShouldBe(order.Id);
        result.Orders[0].CreatedAt.ShouldBe(order.CreatedAt);
        result.Orders[0].Products.ShouldHaveSingleItem();
        result.Orders[0].Products[0].ProductName.ShouldBe("Widget");
        result.Orders[0].Products[0].Quantity.ShouldBe(2);
        result.Orders[0].Products[0].UnitPrice.ShouldBe(10m);
    }

    [Fact]
    public async Task Handler_GetMyOrders_WhenProductNoLongerExists_ShouldFallBackToUnknownProduct()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var order = new Order(Guid.NewGuid(), userId, "Australia", "Melbourne", "1 Main St", "0400000000");
        order.AddProduct(Guid.NewGuid(), 1, 5m);
        orders.Setup(o => o.GetByUserAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([order]);
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetOrdersByUserQueryHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new GetOrdersByUserQuery(userId), default);

        // Assert
        result.Orders[0].Products[0].ProductName.ShouldBe("Unknown product");
    }
}
