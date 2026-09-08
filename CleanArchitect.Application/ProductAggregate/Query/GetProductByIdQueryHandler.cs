using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Query;

public sealed class GetProductByIdQueryHandler(IProductRepository products) : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await products.FindAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.Id}' was not found.");

        return ProductResponse.From(product);
    }
}
