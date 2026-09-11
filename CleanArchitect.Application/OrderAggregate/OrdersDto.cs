namespace CleanArchitect.Application.OrderAggregate;

public class OrdersDto
{
    public OrdersDto(IReadOnlyList<OrderDto> orders)
    {
        Orders = orders;
    }

    public IReadOnlyList<OrderDto> Orders { get; }
}
