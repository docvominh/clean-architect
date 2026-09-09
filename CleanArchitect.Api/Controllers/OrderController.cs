using System.Security.Claims;

using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Command;
using CleanArchitect.Application.OrderAggregate.Query;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
[Authorize]
public class OrderController(ISender sender) : ControllerBase
{
    [HttpPost]
    public Task<OrderDto> Create(OrderRequest request, CancellationToken cancellationToken)
    {
        var createdBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return sender.Send(new CreateOrderCommand(request, createdBy), cancellationToken);
    }

    [HttpGet("mine")]
    public Task<OrdersDto> GetMine(CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return sender.Send(new GetOrdersByUserQuery(userId), cancellationToken);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public Task<OrdersDto> GetAll(CancellationToken cancellationToken)
    {
        return sender.Send(new GetAllOrdersQuery(), cancellationToken);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    public Task<OrderDto> UpdateStatus(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken)
    {
        return sender.Send(new UpdateOrderStatusCommand(id, request.Status), cancellationToken);
    }
}
