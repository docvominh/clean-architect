namespace CleanArchitect.Application.OrderAggregate.Query;

public class OrdersDto
{
    public OrdersDto(IReadOnlyList<OrderDto> orders)
    {
        Orders = orders;
    }

    public IReadOnlyList<OrderDto> Orders { get; }
}
