using CleanArchitect.Application.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed class GetOrdersByUserQueryHandler(IOrderRepository orders, IProductRepository products) : IRequestHandler<GetOrdersByUserQuery, OrdersDto>
{
    public async Task<OrdersDto> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        var userOrders = await orders.GetByUserAsync(request.UserId, cancellationToken);
        var productNames = (await products.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id, p => p.Name);

        var result = userOrders
            .Select(order => new OrderDto(
                order.Id,
                order.Status,
                order.TotalAmount,
                order.CreatedAt,
                order.OrderProducts
                    .Select(orderProduct =>
                        new OrderProductDto(
                            orderProduct.ProductId,
                            productNames.GetValueOrDefault(orderProduct.ProductId, "Unknown product"),
                            orderProduct.Quantity,
                            orderProduct.UnitPrice))
                    .ToList()))
            .ToList();

        return new OrdersDto(result);
    }
}
