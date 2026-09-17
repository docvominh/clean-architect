using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Query;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.OrderAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class GetAllOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> orders = new();
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_GetAllOrders_ShouldReturnOrdersFromEveryUserWithShippingAddress()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 10m);
        var firstOrder = new Order(Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne", "1 Main St", "0400000000", "VIC");
        firstOrder.AddProduct(product.Id, 1, 10m);

        var secondOrder = new Order(Guid.NewGuid(), Guid.NewGuid(), "New Zealand", "Auckland", "2 Other St", "0400000001");
        secondOrder.AddProduct(product.Id, 2, 10m);
        orders.Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([firstOrder, secondOrder]);

        products.Setup(p => p.GetByIdsAsync(It.Is<IReadOnlyList<Guid>>(ids => ids.Count == 1 && ids[0] == product.Id), default))
            .ReturnsAsync([product]);
        var handler = new GetAllOrdersQueryHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new GetAllOrdersQuery(), default);

        // Assert
        result.Orders.Count.ShouldBe(2);
        var mappedFirst = result.Orders.Single(o => o.Id == firstOrder.Id);
        mappedFirst.Country.ShouldBe("Australia");
        mappedFirst.State.ShouldBe("VIC");
        mappedFirst.City.ShouldBe("Melbourne");
        mappedFirst.Products[0].ProductName.ShouldBe("Widget");
        products.Verify(p => p.GetByIdsAsync(It.Is<IReadOnlyList<Guid>>(ids => ids.Count == 1 && ids[0] == product.Id), default), Times.Once);
        products.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handler_GetAllOrdersWhenNoneExist_ShouldReturnEmpty()
    {
        // Arrange
        orders.Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        products.Setup(p => p.GetByIdsAsync(It.Is<IReadOnlyList<Guid>>(ids => ids.Count == 0), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetAllOrdersQueryHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new GetAllOrdersQuery(), default);

        // Assert
        result.Orders.ShouldBeEmpty();
        products.Verify(p => p.GetByIdsAsync(It.Is<IReadOnlyList<Guid>>(ids => ids.Count == 0), It.IsAny<CancellationToken>()), Times.Once);
        products.Verify(p => p.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handler_WithMissingProduct_ShouldUseUnknownProductName()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne", "1 Main St", "0400000000");
        var productId = Guid.NewGuid();
        order.AddProduct(productId, 2, 10m);
        orders.Setup(o => o.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([order]);
        products.Setup(p => p.GetByIdsAsync(It.Is<IReadOnlyList<Guid>>(ids => ids.Count == 1 && ids[0] == productId), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var handler = new GetAllOrdersQueryHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new GetAllOrdersQuery(), default);

        // Assert
        result.Orders.ShouldHaveSingleItem().Products.ShouldBe([new OrderProductDto(productId, "Unknown product", 2, 10m)]);
    }
}
