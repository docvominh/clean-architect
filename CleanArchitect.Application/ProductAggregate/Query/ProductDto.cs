using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate.Query;

public class ProductDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Manufacturer { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? PriceDiscount { get; init; }

    public static ProductDto From(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Manufacturer = product.Manufacturer,
            ImageUrl = product.ImageUrl,
            Description = product.Description,
            Price = product.Price,
            PriceDiscount = product.PriceDiscount
        };
    }
}
