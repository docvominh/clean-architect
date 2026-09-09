using System.ComponentModel.DataAnnotations;

using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

public sealed record UpdateOrderStatusRequest
{
    [Required] public required OrderStatus Status { get; init; }
}
