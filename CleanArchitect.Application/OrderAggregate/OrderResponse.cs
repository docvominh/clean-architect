using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

public sealed record OrderResponse(Guid Id, OrderStatus Status, decimal TotalAmount, IReadOnlyList<OrderProductResponse> Products);

public sealed record OrderProductResponse(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
