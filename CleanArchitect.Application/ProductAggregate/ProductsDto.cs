namespace CleanArchitect.Application.ProductAggregate;

public class ProductsDto
{
    public ProductsDto(IReadOnlyList<ProductDto> products)
    {
        Products = products;
    }

    public IReadOnlyList<ProductDto> Products { get; }
}
