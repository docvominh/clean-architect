using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Command;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.OrderAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class UpdateOrderStatusCommandHandlerTests
{
    private readonly Mock<IOrderRepository> orders = new();
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_UpdateOrderStatus_ShouldChangeStatusAndSave()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne", "1 Main St", "0400000000");
        orders.Setup(o => o.FindAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new UpdateOrderStatusCommandHandler(orders.Object, products.Object);

        // Act
        var result = await handler.Handle(new UpdateOrderStatusCommand(order.Id, OrderStatus.Shipped), default);

        // Assert
        result.Status.ShouldBe(OrderStatus.Shipped);
        order.Status.ShouldBe(OrderStatus.Shipped);
        orders.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_UpdateMissingOrderStatus_ShouldThrowNotFoundException()
    {
        // Arrange
        orders.Setup(o => o.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
        var handler = new UpdateOrderStatusCommandHandler(orders.Object, products.Object);

        // Act
        var act = () => handler.Handle(new UpdateOrderStatusCommand(Guid.NewGuid(), OrderStatus.Shipped), default);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
        orders.Verify(o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
