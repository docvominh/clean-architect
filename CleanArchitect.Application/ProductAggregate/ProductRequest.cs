using System.ComponentModel.DataAnnotations;

namespace CleanArchitect.Application.ProductAggregate;

public sealed record ProductRequest
{
    [Required] public required string Name { get; init; }

    [Required] public required string Manufacturer { get; init; }

    public string? ImageUrl { get; init; }

    public string? Description { get; init; }

    [Range(0, double.MaxValue)] public decimal Price { get; init; }

    public decimal? PriceDiscount { get; init; }
}
