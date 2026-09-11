using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public static class ProductConverter
{
    public static ProductDto ToProductDto(Product product)
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

    public static ProductsDto ToProductsDto(IReadOnlyCollection<Product> products)
    {
        return new ProductsDto(products.Select(ToProductDto).ToList());
    }
}
