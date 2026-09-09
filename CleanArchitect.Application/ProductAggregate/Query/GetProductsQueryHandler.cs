using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Query;

public sealed class GetProductsQueryHandler(IProductRepository products) : IRequestHandler<GetProductsQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var all = await products.GetAllAsync(cancellationToken);

        return all.Select(ProductResponse.From).ToList();
    }
}
