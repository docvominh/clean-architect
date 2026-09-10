using CleanArchitect.Domain.ProductAggregate;
using Shouldly;
using Xunit;

namespace CleanArchitect.Application.UnitTests.ProductAggregate;

public class ProductTests
{
    [Theory]
    [InlineData(null, "Maker", 10, null, "name")]
    [InlineData("", "Maker", 10, null, "name")]
    [InlineData(" ", "Maker", 10, null, "name")]
    [InlineData("Name", null, 10, null, "manufacturer")]
    [InlineData("Name", "", 10, null, "manufacturer")]
    [InlineData("Name", " ", 10, null, "manufacturer")]
    [InlineData("Name", "Maker", -1, null, "price")]
    [InlineData("Name", "Maker", 10, -1, "priceDiscount")]
    [InlineData("Name", "Maker", 10, 11, "priceDiscount")]
    public void Constructor_RejectsInvalidValues(string? name, string? manufacturer, int price, int? discount, string parameter)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => _ = new Product(id, id, name!, manufacturer!, price, priceDiscount: discount);

        // Assert
        Should.Throw<ArgumentException>(act).ParamName.ShouldBe(parameter);
    }

    [Theory]
    [InlineData(0, null)]
    [InlineData(0, 0)]
    [InlineData(10, 0)]
    [InlineData(10, 10)]
    public void Constructor_AcceptsPriceBoundaries(int price, int? discount)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var product = new Product(id, id, "Name", "Maker", price, priceDiscount: discount);

        // Assert
        product.Price.ShouldBe(price);
        product.PriceDiscount.ShouldBe(discount is null ? null : (decimal?)discount.Value);
    }

    [Fact]
    public void UpdateDetails_WhenInvalid_PreservesExistingState()
    {
        // Arrange
        var creator = Guid.NewGuid();
        var product = new Product(Guid.NewGuid(), creator, "Original", "Maker", 10, priceDiscount: 5);

        // Act
        Action act = () => product.UpdateDetails(Guid.NewGuid(), "Changed", "Other", 2, priceDiscount: 3);

        // Assert
        Should.Throw<ArgumentOutOfRangeException>(act).ParamName.ShouldBe("priceDiscount");
        product.Name.ShouldBe("Original");
        product.Manufacturer.ShouldBe("Maker");
        product.Price.ShouldBe(10);
        product.PriceDiscount.ShouldBe(5);
        product.UpdateBy.ShouldBe(creator);
    }
}
