namespace CleanArchitect.Application.ProductAggregate;

public sealed record ProductDto(
    Guid Id,
    string Name,
    string Manufacturer,
    string? ImageUrl,
    string? Description,
    decimal Price,
    decimal? PriceDiscount);
