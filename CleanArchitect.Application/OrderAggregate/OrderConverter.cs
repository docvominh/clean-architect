using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

public static class OrderConverter
{
    public static OrderDto ToOrderDto(Order order, IReadOnlyDictionary<Guid, string> productNames)
    {
        return new OrderDto(
            order.Id,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Country,
            order.State,
            order.City,
            order.Street,
            order.ContactPhoneNumber,
            order.OrderProducts
                .Select(op => new OrderProductDto(op.ProductId, productNames.GetValueOrDefault(op.ProductId, "Unknown product"), op.Quantity, op.UnitPrice))
                .ToList());
    }

    public static OrdersDto ToOrdersDto(IReadOnlyCollection<Order> orders, IReadOnlyDictionary<Guid, string> productNames)
    {
        return new OrdersDto(orders.Select(order => ToOrderDto(order, productNames)).ToList());
    }
}
