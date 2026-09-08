namespace CleanArchitect.Domain.OrderAggregate;

public class Order : BaseEntity
{
    public Order(
        Guid id,
        Guid createBy,
        string country,
        string city,
        string street,
        string contactPhoneNumber,
        string? state = null,
        OrderStatus status = OrderStatus.Pending,
        decimal totalAmount = 0m) : base(id, createBy)
    {
        Country = country;
        State = state;
        City = city;
        Street = street;
        ContactPhoneNumber = contactPhoneNumber;
        Status = status;
        TotalAmount = totalAmount;
    }

    public string Country { get; init; }

    public string? State { get; init; }

    public string City { get; init; }

    public string Street { get; init; }

    public string ContactPhoneNumber { get; init; }

    public OrderStatus Status { get; init; }

    public decimal TotalAmount { get; init; }

    public List<OrderProduct> OrderProducts { get; init; } = [];
}
