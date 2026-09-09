using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Command;

public sealed record CreateOrderCommand(OrderRequest Request, Guid CreatedBy) : IRequest<OrderResponse>;
