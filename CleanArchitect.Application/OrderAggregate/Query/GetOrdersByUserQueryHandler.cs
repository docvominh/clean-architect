using CleanArchitect.Application.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed class GetOrdersByUserQueryHandler(IOrderRepository orders, IProductRepository products) : IRequestHandler<GetOrdersByUserQuery, OrdersDto>
{
    public async Task<OrdersDto> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        var userOrders = await orders.GetByUserAsync(request.UserId, cancellationToken);
        var productNames = (await products.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id, p => p.Name);

        return OrderConverter.ToOrdersDto(userOrders, productNames);
    }
}
