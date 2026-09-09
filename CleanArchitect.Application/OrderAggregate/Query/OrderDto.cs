using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed record OrderDto(Guid Id, OrderStatus Status, decimal TotalAmount, DateTimeOffset CreatedAt, IReadOnlyList<OrderProductDto> Products);

public sealed record OrderProductDto(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
