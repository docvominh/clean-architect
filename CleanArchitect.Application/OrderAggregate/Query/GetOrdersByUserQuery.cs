using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed record GetOrdersByUserQuery(Guid UserId) : IRequest<OrdersDto>;
