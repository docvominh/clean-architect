using CleanArchitect.Domain.OrderAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Command;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest<OrderDto>;
