using CleanArchitect.Application.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed class GetAllOrdersQueryHandler(IOrderRepository orders, IProductRepository products) : IRequestHandler<GetAllOrdersQuery, OrdersDto>
{
    public async Task<OrdersDto> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var allOrders = await orders.GetAllAsync(cancellationToken);
        var productNames = (await products.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id, p => p.Name);

        return new OrdersDto(allOrders.Select(o => OrderDto.From(o, productNames)).ToList());
    }
}
