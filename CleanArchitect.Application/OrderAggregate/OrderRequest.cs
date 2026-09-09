using System.ComponentModel.DataAnnotations;

namespace CleanArchitect.Application.OrderAggregate;

public sealed record OrderRequest
{
    [Required] public required string Country { get; init; }

    public string? State { get; init; }

    [Required] public required string City { get; init; }

    [Required] public required string Street { get; init; }

    [Required] public required string ContactPhoneNumber { get; init; }

    [Required, MinLength(1)] public required List<OrderItemRequest> Items { get; init; }
}

public sealed record OrderItemRequest
{
    [Required] public required Guid ProductId { get; init; }

    [Range(1, int.MaxValue)] public int Quantity { get; init; }
}
