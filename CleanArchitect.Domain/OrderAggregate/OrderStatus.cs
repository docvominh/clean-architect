namespace CleanArchitect.Domain.OrderAggregate;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Shipped,
    Delivered,
    Cancelled,
}
