namespace CleanArchitect.Domain.ProductAggregate;

public class Product : BaseEntity
{
    public Product(
        Guid id,
        Guid createBy,
        string name,
        string manufacturer,
        decimal price,
        string? imageUrl = null,
        string? description = null,
        decimal? priceDiscount = null) : base(id, createBy)
    {
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        ImageUrl = imageUrl;
        Description = description;
        PriceDiscount = priceDiscount;
    }

    public string? ImageUrl { get; private set; }

    public string Name { get; private set; }

    public string Manufacturer { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public decimal? PriceDiscount { get; private set; }

    public void UpdateDetails(
        Guid updateBy,
        string name,
        string manufacturer,
        decimal price,
        string? imageUrl = null,
        string? description = null,
        decimal? priceDiscount = null)
    {
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        ImageUrl = imageUrl;
        Description = description;
        PriceDiscount = priceDiscount;
        MarkUpdated(updateBy);
    }
}
