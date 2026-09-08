using CleanArchitect.Application;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Command;
using CleanArchitect.Domain.ProductAggregate;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> products = new();

    private static ProductRequest Request(string name = "Widget", decimal price = 9.99m) => new()
    {
        Name = name,
        Manufacturer = "Acme",
        Price = price,
    };

    [Fact]
    public async Task Handler_UpdateProduct_ShouldApplyChangesAndRecordUpdater()
    {
        var creator = Guid.NewGuid();
        var updater = Guid.NewGuid();
        var product = new Product(Guid.NewGuid(), creator, "Old Name", "Old Manufacturer", 1m);
        products.Setup(p => p.FindAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new UpdateProductCommandHandler(products.Object);
        var result = await handler.Handle(new UpdateProductCommand(product.Id, Request("New Name", 5m), updater), default);

        result.Name.ShouldBe("New Name");
        result.Price.ShouldBe(5m);
        product.UpdateBy.ShouldBe(updater);
        products.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handler_UpdateMissingProduct_ShouldThrowNotFoundException()
    {
        products.Setup(p => p.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        var handler = new UpdateProductCommandHandler(products.Object);

        await Should.ThrowAsync<NotFoundException>(() => handler.Handle(new UpdateProductCommand(Guid.NewGuid(), Request(), Guid.NewGuid()), default));

        products.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
