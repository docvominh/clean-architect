using CleanArchitect.Domain.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed class CreateProductCommandHandler(IProductRepository products) : IRequestHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            request.CreatedBy,
            request.Request.Name,
            request.Request.Manufacturer,
            request.Request.Price,
            request.Request.ImageUrl,
            request.Request.Description,
            request.Request.PriceDiscount);

        products.Add(product);
        await products.SaveChangesAsync(cancellationToken);

        return ProductResponse.From(product);
    }
}
