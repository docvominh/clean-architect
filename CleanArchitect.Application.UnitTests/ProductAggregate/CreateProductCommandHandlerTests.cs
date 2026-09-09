using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Command;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> products = new();

    private static ProductRequest Request(string name = "Widget", decimal price = 9.99m)
    {
        return new ProductRequest
        {
            Name = name,
            Manufacturer = "Acme",
            Price = price
        };
    }

    [Fact]
    public async Task Handler_CreateProduct_ShouldAddProductOwnedByCreator()
    {
        // Arrange
        var createdBy = Guid.NewGuid();
        var handler = new CreateProductCommandHandler(products.Object);

        // Act
        var result = await handler.Handle(new CreateProductCommand(Request(), createdBy), default);

        // Assert
        products.Verify(
            p => p.Add(
                It.Is<Product>(x =>
                    x.Name == "Widget" && x.Manufacturer == "Acme" && x.Price == 9.99m && x.CreateBy == createdBy)), Times.Once);
        products.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Name.ShouldBe("Widget");
    }
}
