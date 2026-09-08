using CleanArchitect.Domain.ProductAggregate;
using CleanArchitect.Infrastructure.IntegrationTests.Fixtures;
using CleanArchitect.Infrastructure.ProductAggregate;

using Microsoft.EntityFrameworkCore;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.ProductAggregate;

[Collection("MsSql")]
public sealed class ProductRepositoryTests(MsSqlContainerFixture fixture) : RepositoryTestBase(fixture)
{
    private ProductRepository Repository => new(Context);

    private static Product CreateProduct(string name = "Keyboard")
    {
        return new Product(
            Guid.NewGuid(), Guid.NewGuid(), name, "Acme", 123.45m,
            "https://example.com/product.png", "Mechanical keyboard", 99.95m);
    }

    [Fact]
    public async Task AddAndSaveChanges_PersistsAllFieldsAndAuditValues()
    {
        // Arrange
        var product = CreateProduct();

        // Act
        Repository.Add(product);
        bool wasMissingBeforeSave;
        await using (var beforeSave = CreateDbContext())
        {
            wasMissingBeforeSave = await new ProductRepository(beforeSave).FindAsync(product.Id, CancellationToken.None) is null;
        }

        await Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        wasMissingBeforeSave.ShouldBeTrue();
        await using var verification = CreateDbContext();
        var saved = await new ProductRepository(verification).FindAsync(product.Id, CancellationToken.None);
        saved.ShouldNotBeNull();
        saved.Name.ShouldBe(product.Name);
        saved.Manufacturer.ShouldBe(product.Manufacturer);
        saved.Description.ShouldBe(product.Description);
        saved.ImageUrl.ShouldBe(product.ImageUrl);
        saved.Price.ShouldBe(123.45m);
        saved.PriceDiscount.ShouldBe(99.95m);
        saved.CreateBy.ShouldBe(product.CreateBy);
        saved.UpdateBy.ShouldBe(product.CreateBy);
        saved.CreatedAt.ShouldNotBe(default);
        saved.ModifiedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task FindAsync_ReturnsNullForMissingProduct()
    {
        // Arrange
        // The fixture provides an empty database and a fresh context.

        // Act
        var result = await Repository.FindAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyForEmptyDatabase()
    {
        // Arrange
        // The fixture provides an empty database and a fresh context.

        // Act
        var result = await Repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProductsWithoutTracking()
    {
        // Arrange
        var first = CreateProduct("First");
        var second = CreateProduct("Second");
        Repository.Add(first);
        Repository.Add(second);
        await Repository.SaveChangesAsync(CancellationToken.None);

        await using var reading = CreateDbContext();

        // Act
        var products = await new ProductRepository(reading).GetAllAsync(CancellationToken.None);

        // Assert
        products.Count.ShouldBe(2);
        products.ShouldContain(p => p.Id == first.Id);
        products.ShouldContain(p => p.Id == second.Id);
        reading.ChangeTracker.Entries<Product>().ShouldBeEmpty();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsTrackedUpdatesAndClearsOptionalFields()
    {
        // Arrange
        var product = CreateProduct();
        Repository.Add(product);
        await Repository.SaveChangesAsync(CancellationToken.None);
        var createdAt = product.CreatedAt;
        var previousModifiedAt = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Persist an old timestamp so the audit check does not depend on clock resolution or a delay.
        await Context.Products.Where(p => p.Id == product.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.ModifiedAt, previousModifiedAt));
        Context.ChangeTracker.Clear();
        var loaded = await Repository.FindAsync(product.Id, CancellationToken.None);
        loaded.ShouldNotBeNull();
        var editor = Guid.NewGuid();

        // Act
        loaded.UpdateDetails(editor, "Updated", "New manufacturer", 42.12m);
        await Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        await using var verification = CreateDbContext();
        var saved = await verification.Products.SingleAsync();
        saved.Name.ShouldBe("Updated");
        saved.Manufacturer.ShouldBe("New manufacturer");
        saved.Price.ShouldBe(42.12m);
        saved.Description.ShouldBeNull();
        saved.ImageUrl.ShouldBeNull();
        saved.PriceDiscount.ShouldBeNull();
        saved.UpdateBy.ShouldBe(editor);
        saved.CreatedAt.ShouldBe(createdAt);
        saved.ModifiedAt.ShouldBeGreaterThan(previousModifiedAt);
    }

    [Fact]
    public async Task RemoveAndSaveChanges_DeletesOnlySelectedProduct()
    {
        // Arrange
        var removed = CreateProduct("Remove");
        var retained = CreateProduct("Keep");
        Repository.Add(removed);
        Repository.Add(retained);
        await Repository.SaveChangesAsync(CancellationToken.None);
        Context.ChangeTracker.Clear();
        var loaded = await Repository.FindAsync(removed.Id, CancellationToken.None);
        loaded.ShouldNotBeNull();

        // Act
        Repository.Remove(loaded);
        await Repository.SaveChangesAsync(CancellationToken.None);

        // Assert
        await using var verification = CreateDbContext();
        (await verification.Products.SingleAsync()).Id.ShouldBe(retained.Id);
    }
}
