using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Query;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_GetAllProducts_ShouldMapEveryProduct()
    {
        // Arrange
        var list = new List<Product>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 9.99m),
            new(Guid.NewGuid(), Guid.NewGuid(), "Gadget", "Acme", 19.99m)
        };
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
        var handler = new GetProductsQueryHandler(products.Object);

        // Act
        var result = await handler.Handle(new GetProductsQuery(), default);

        // Assert
        result.Count.ShouldBe(2);
        result.Select(r => r.Name).ShouldBe(["Widget", "Gadget"]);
    }

    [Fact]
    public async Task Handler_GetAllProductsWhenNoneExist_ShouldReturnEmptyList()
    {
        // Arrange
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetProductsQueryHandler(products.Object);

        // Act
        var result = await handler.Handle(new GetProductsQuery(), default);

        // Assert
        result.ShouldBeEmpty();
    }
}
