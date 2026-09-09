using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Command;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.OrderAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> orders = new();
    private readonly Mock<IProductRepository> products = new();

    private static OrderRequest Request(Guid productId, int quantity = 2) => new()
    {
        Country = "Australia",
        City = "Melbourne",
        Street = "1 Main St",
        ContactPhoneNumber = "0400000000",
        Items = [new OrderItemRequest { ProductId = productId, Quantity = quantity }],
    };

    [Fact]
    public async Task Handler_CreateOrder_ShouldAddOrderWithServerComputedPrice()
    {
        // Arrange
        var createdBy = Guid.NewGuid();
        var product = new Product(Guid.NewGuid(), createdBy, "Widget", "Acme", price: 100m, priceDiscount: 80m);
        products.Setup(p => p.FindAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new CreateOrderCommandHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new CreateOrderCommand(Request(product.Id, quantity: 3), createdBy), default);

        // Assert
        orders.Verify(
            o => o.Add(
                It.Is<Order>(x =>
                    x.CreateBy == createdBy
                    && x.Country == "Australia"
                    && x.City == "Melbourne"
                    && x.OrderProducts.Count == 1
                    && x.OrderProducts[0].ProductId == product.Id
                    && x.OrderProducts[0].Quantity == 3
                    && x.OrderProducts[0].UnitPrice == 80m
                    && x.TotalAmount == 240m)), Times.Once);
        orders.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.TotalAmount.ShouldBe(240m);
        result.Products.ShouldHaveSingleItem();
        result.Products[0].UnitPrice.ShouldBe(80m);
    }

    [Fact]
    public async Task Handler_CreateOrder_WhenProductMissing_ShouldThrowAndNotSave()
    {
        // Arrange
        var createdBy = Guid.NewGuid();
        var missingProductId = Guid.NewGuid();
        products.Setup(p => p.FindAsync(missingProductId, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var handler = new CreateOrderCommandHandler(orders.Object, products.Object);

        // Act
        var act = () => handler.Handle(new CreateOrderCommand(Request(missingProductId), createdBy), default);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        orders.Verify(o => o.Add(It.IsAny<Order>()), Times.Never);
        orders.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
