using System.Security.Claims;

using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Application.OrderAggregate.Command;

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
    public Task<OrderResponse> Create(OrderRequest request, CancellationToken cancellationToken)
    {
        var createdBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return sender.Send(new CreateOrderCommand(request, createdBy), cancellationToken);
    }
}
