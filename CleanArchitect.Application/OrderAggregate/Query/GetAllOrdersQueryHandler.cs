using CleanArchitect.Application.ProductAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed class GetAllOrdersQueryHandler(IOrderRepository orders, IProductRepository productRepository) : IRequestHandler<GetAllOrdersQuery, OrdersDto>
{
    public async Task<OrdersDto> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var allOrders = await orders.GetAllAsync(cancellationToken);
        var productIds = allOrders.SelectMany(order => order.OrderProducts).Select(product => product.ProductId).Distinct().ToList();
        var productNames = (await productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id, p => p.Name);

        return OrderConverter.ToOrdersDto(allOrders, productNames);
    }
}
