using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed record OrderDto(
    Guid Id,
    OrderStatus Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    string Country,
    string? State,
    string City,
    string Street,
    string ContactPhoneNumber,
    IReadOnlyList<OrderProductDto> Products)
{
    public static OrderDto From(Order order, IReadOnlyDictionary<Guid, string> productNames)
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
}

public sealed record OrderProductDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
