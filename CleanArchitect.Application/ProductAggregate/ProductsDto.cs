namespace CleanArchitect.Application.ProductAggregate;

public class ProductsDto
{
    public ProductsDto(IReadOnlyCollection<ProductDto> products)
    {
        Products = products;
    }

    public IReadOnlyCollection<ProductDto> Products { get; }
}
