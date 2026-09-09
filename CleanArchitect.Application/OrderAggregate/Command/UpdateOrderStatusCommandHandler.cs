using CleanArchitect.Application.OrderAggregate.Query;
using CleanArchitect.Application.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Command;

public sealed class UpdateOrderStatusCommandHandler(IOrderRepository orders, IProductRepository products) : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
{
    public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orders.FindAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException($"Order '{request.OrderId}' was not found.");

        order.UpdateStatus(request.Status);
        await orders.SaveChangesAsync(cancellationToken);

        var productNames = (await products.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id, p => p.Name);

        return OrderDto.From(order, productNames);
    }
}
