using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed class DeleteProductCommandHandler(IProductRepository products) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await products.FindAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Product '{request.Id}' was not found.");

        products.Remove(product);
        await products.SaveChangesAsync(cancellationToken);
    }
}
