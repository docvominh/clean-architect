using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public static class ProductConverter
{
    public static ProductDto ToProductDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Manufacturer,
            product.ImageUrl,
            product.Description,
            product.Price,
            product.PriceDiscount);
    }

    public static ProductsDto ToProductsDto(IReadOnlyCollection<Product> products)
    {
        return new ProductsDto(products.Select(ToProductDto).ToList());
    }
}
