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
        var list = new List<Product>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 9.99m),
            new(Guid.NewGuid(), Guid.NewGuid(), "Gadget", "Acme", 19.99m),
        };
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);
        var handler = new GetProductsQueryHandler(products.Object);

        var result = await handler.Handle(new GetProductsQuery(), default);

        result.Count.ShouldBe(2);
        result.Select(r => r.Name).ShouldBe(["Widget", "Gadget"]);
    }

    [Fact]
    public async Task Handler_GetAllProductsWhenNoneExist_ShouldReturnEmptyList()
    {
        products.Setup(p => p.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetProductsQueryHandler(products.Object);

        var result = await handler.Handle(new GetProductsQuery(), default);

        result.ShouldBeEmpty();
    }
}
