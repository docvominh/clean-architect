using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Manufacturer,
    string? ImageUrl,
    string? Description,
    decimal Price,
    decimal? PriceDiscount)
{
    public static ProductResponse From(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Manufacturer,
            product.ImageUrl,
            product.Description,
            product.Price,
            product.PriceDiscount);
    }
}
