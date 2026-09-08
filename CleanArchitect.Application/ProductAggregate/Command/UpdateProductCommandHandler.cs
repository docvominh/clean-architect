using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed class UpdateProductCommandHandler(IProductRepository products) : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await products.FindAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.Id}' was not found.");

        product.UpdateDetails(
            request.UpdatedBy,
            request.Request.Name,
            request.Request.Manufacturer,
            request.Request.Price,
            request.Request.ImageUrl,
            request.Request.Description,
            request.Request.PriceDiscount);

        await products.SaveChangesAsync(cancellationToken);

        return ProductResponse.From(product);
    }
}
