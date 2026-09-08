namespace CleanArchitect.Domain.Product;

public class Product : BaseEntity
{
    public string? ImageUrl { get; set; }

    public required string Name { get; set; }

    public required string Manufacturer { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? PriceDiscount { get; set; }
}
