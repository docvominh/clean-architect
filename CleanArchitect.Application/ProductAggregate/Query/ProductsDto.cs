namespace CleanArchitect.Application.ProductAggregate.Query;

public class ProductsDto
{
    public ProductsDto(IReadOnlyList<ProductDto> products)
    {
        Products = products;
    }

    public IReadOnlyList<ProductDto> Products { get; }
}
