using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Query;

public sealed class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, ProductsDto>
{
    public async Task<ProductsDto> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);

        return new ProductsDto(products.Select(ProductDto.From).ToList());
    }
}
