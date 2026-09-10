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
        Validate(name, manufacturer, price, priceDiscount);

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
        Validate(name, manufacturer, price, priceDiscount);

        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        ImageUrl = imageUrl;
        Description = description;
        PriceDiscount = priceDiscount;
        MarkUpdated(updateBy);
    }

    private static void Validate(string name, string manufacturer, decimal price, decimal? priceDiscount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(manufacturer);
        ArgumentOutOfRangeException.ThrowIfNegative(price);

        if (priceDiscount is decimal discount && (discount < 0 || discount > price))
        {
            throw new ArgumentOutOfRangeException(nameof(priceDiscount), "Discounted price must be between zero and the regular price.");
        }
    }
}
