using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class ProductConverterTest
{
    [Fact]
    public void ToProductDto_ShouldMapEveryProperty()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 29.99m,
            "https://example.com/widget.png", "A useful widget", 19.99m);

        // Act
        var result = ProductConverter.ToProductDto(product);

        // Assert
        result.Id.ShouldBe(product.Id);
        result.Name.ShouldBe("Widget");
        result.Manufacturer.ShouldBe("Acme");
        result.ImageUrl.ShouldBe("https://example.com/widget.png");
        result.Description.ShouldBe("A useful widget");
        result.Price.ShouldBe(29.99m);
        result.PriceDiscount.ShouldBe(19.99m);
    }

    [Fact]
    public void ToProductDto_WithoutOptionalValues_ShouldPreserveNullsAndZeroPrice()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), Guid.NewGuid(), "Sample", "Contoso", 0m);

        // Act
        var result = ProductConverter.ToProductDto(product);

        // Assert
        result.Id.ShouldBe(product.Id);
        result.Name.ShouldBe("Sample");
        result.Manufacturer.ShouldBe("Contoso");
        result.ImageUrl.ShouldBeNull();
        result.Description.ShouldBeNull();
        result.Price.ShouldBe(0m);
        result.PriceDiscount.ShouldBeNull();
    }

    [Fact]
    public void ToProductsDto_ShouldMapEveryProductInOrder()
    {
        // Arrange
        Product[] products =
        [
            new(
                Guid.NewGuid(), Guid.NewGuid(), "Widget", "Acme", 29.99m,
                "https://example.com/widget.png", "A useful widget", 19.99m),
            new(
                Guid.NewGuid(), Guid.NewGuid(), "Gadget", "Contoso", 49.99m,
                "https://example.com/gadget.png", "A useful gadget", 39.99m)
        ];

        // Act
        var result = ProductConverter.ToProductsDto(products);

        // Assert
        result.Products.Count.ShouldBe(2);

        for (var index = 0; index < products.Length; index++)
        {
            var expected = products[index];
            var actual = result.Products[index];
            actual.Id.ShouldBe(expected.Id);
            actual.Name.ShouldBe(expected.Name);
            actual.Manufacturer.ShouldBe(expected.Manufacturer);
            actual.ImageUrl.ShouldBe(expected.ImageUrl);
            actual.Description.ShouldBe(expected.Description);
            actual.Price.ShouldBe(expected.Price);
            actual.PriceDiscount.ShouldBe(expected.PriceDiscount);
        }
    }

    [Fact]
    public void ToProductsDto_WithEmptyCollection_ShouldReturnEmptyList()
    {
        // Arrange
        Product[] products = [];

        // Act
        var result = ProductConverter.ToProductsDto(products);

        // Assert
        result.Products.ShouldBeEmpty();
    }

    [Fact]
    public void ToProductsDto_WithNullCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        IReadOnlyCollection<Product> products = null!;

        // Act
        void Action()
        {
            ProductConverter.ToProductsDto(products);
        }

        // Assert
        Should.Throw<ArgumentNullException>((Action)Action);
    }
}
