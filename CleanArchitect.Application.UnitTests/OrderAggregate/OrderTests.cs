using CleanArchitect.Domain.OrderAggregate;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.OrderAggregate;

public class OrderTests
{
    private static Order CreateOrder(Guid createBy, Guid productId, int quantity = 1, decimal unitPrice = 10m)
    {
        var order = new Order(Guid.NewGuid(), createBy, "Australia", "Melbourne", "1 Main St", "0400000000");
        order.AddProduct(productId, quantity, unitPrice);

        return order;
    }

    [Fact]
    public void AddProduct_ShouldAppendLineAndAccumulateTotal()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), "Australia", "Melbourne", "1 Main St", "0400000000");

        // Act
        order.AddProduct(Guid.NewGuid(), 2, 15m);
        order.AddProduct(Guid.NewGuid(), 1, 5m);

        // Assert
        order.OrderProducts.Count.ShouldBe(2);
        order.TotalAmount.ShouldBe(35m);
    }

    [Fact]
    public void UpdateProduct_WhenProductInOrder_ShouldReplaceLineAndRecalculateTotal()
    {
        // Arrange
        var createBy = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var order = CreateOrder(createBy, productId);

        // Act
        order.UpdateProduct(productId, 3, 12m);

        // Assert
        order.OrderProducts.ShouldHaveSingleItem();
        order.OrderProducts[0].Quantity.ShouldBe(3);
        order.OrderProducts[0].UnitPrice.ShouldBe(12m);
        order.TotalAmount.ShouldBe(36m);
    }

    [Fact]
    public void UpdateProduct_WhenProductNotInOrder_ShouldThrow()
    {
        // Arrange
        var createBy = Guid.NewGuid();
        var order = CreateOrder(createBy, Guid.NewGuid());

        // Act
        var act = () => order.UpdateProduct(Guid.NewGuid(), 1, 5m);

        // Assert
        Should.Throw<InvalidOperationException>(act);
    }
}
