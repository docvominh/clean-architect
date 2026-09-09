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

    public decimal TotalAmount { get; private set; }

    public List<OrderProduct> OrderProducts { get; private set; } = [];

    public void AddProduct(Guid productId, int quantity, decimal unitPrice)
    {
        OrderProducts.Add(new OrderProduct(Guid.NewGuid(), CreateBy, Id, productId, quantity, unitPrice));
        TotalAmount += unitPrice * quantity;
    }

    public void UpdateProduct(Guid productId, int quantity, decimal unitPrice)
    {
        var existing = OrderProducts.FirstOrDefault(p => p.ProductId == productId)
            ?? throw new InvalidOperationException($"Order '{Id}' does not contain product '{productId}'.");

        OrderProducts.Remove(existing);
        OrderProducts.Add(new OrderProduct(existing.Id, existing.CreateBy, Id, productId, quantity, unitPrice));
        TotalAmount = OrderProducts.Sum(p => p.UnitPrice * p.Quantity);
    }
}
