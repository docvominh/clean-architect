using CleanArchitect.Application;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Command;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> products = new();

    [Fact]
    public async Task Handler_DeleteExistingProduct_ShouldRemoveProduct()
    {
        var product = new Product(Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 1m);
        products.Setup(p => p.FindAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
        var handler = new DeleteProductCommandHandler(products.Object);

        await handler.Handle(new DeleteProductCommand(product.Id), default);

        products.Verify(p => p.Remove(product), Times.Once);
        products.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_DeleteMissingProduct_ShouldThrowNotFoundException()
    {
        products.Setup(p => p.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var handler = new DeleteProductCommandHandler(products.Object);

        await Should.ThrowAsync<NotFoundException>(() => handler.Handle(new DeleteProductCommand(Guid.NewGuid()), default));

        products.Verify(p => p.Remove(It.IsAny<Product>()), Times.Never);
    }
}
