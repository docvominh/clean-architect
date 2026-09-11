using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

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
    IReadOnlyList<OrderProductDto> Products);

public sealed record OrderProductDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
