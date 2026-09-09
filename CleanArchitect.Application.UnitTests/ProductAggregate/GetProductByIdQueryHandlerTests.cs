using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Query;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_GetProductById_ShouldReturnMappedProduct()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 9.99m);
        products.Setup(p => p.FindAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new GetProductByIdQueryHandler(products.Object);

        // Act
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), default);

        // Assert
        result.Id.ShouldBe(product.Id);
        result.Name.ShouldBe("Widget");
    }

    [Fact]
    public async Task Handler_GetMissingProductById_ShouldThrowNotFoundException()
    {
        // Arrange
        products.Setup(p => p.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var handler = new GetProductByIdQueryHandler(products.Object);

        // Act
        Func<Task> act = () => handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), default);

        // Assert
        await Should.ThrowAsync<NotFoundException>(act);
    }
}
