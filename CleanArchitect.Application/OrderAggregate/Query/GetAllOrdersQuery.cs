using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Query;

public sealed record GetAllOrdersQuery : IRequest<OrdersDto>;
