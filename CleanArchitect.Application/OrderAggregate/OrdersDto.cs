namespace CleanArchitect.Application.OrderAggregate;

public class OrdersDto
{
    public OrdersDto(IReadOnlyCollection<OrderDto> orders)
    {
        Orders = orders;
    }

    public IReadOnlyCollection<OrderDto> Orders { get; }
}
