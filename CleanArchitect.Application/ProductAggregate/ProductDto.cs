using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public class ProductDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Manufacturer { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? PriceDiscount { get; init; }
}
