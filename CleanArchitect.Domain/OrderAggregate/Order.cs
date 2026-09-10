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
        Validate(country, city, street, contactPhoneNumber, status, totalAmount);

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

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public List<OrderProduct> OrderProducts { get; } = [];

    public void UpdateStatus(OrderStatus status)
    {
        ValidateStatus(status);
        Status = status;
    }

    public void AddProduct(Guid productId, int quantity, decimal unitPrice)
    {
        ValidateProduct(quantity, unitPrice);
        OrderProducts.Add(new OrderProduct(Guid.NewGuid(), CreateBy, Id, productId, quantity, unitPrice));
        TotalAmount += unitPrice * quantity;
    }

    public void UpdateProduct(Guid productId, int quantity, decimal unitPrice)
    {
        ValidateProduct(quantity, unitPrice);
        var existing = OrderProducts.FirstOrDefault(p => p.ProductId == productId)
            ?? throw new InvalidOperationException($"Order '{Id}' does not contain product '{productId}'.");

        OrderProducts.Remove(existing);
        OrderProducts.Add(new OrderProduct(existing.Id, existing.CreateBy, Id, productId, quantity, unitPrice));
        TotalAmount = OrderProducts.Sum(p => p.UnitPrice * p.Quantity);
    }

    private static void Validate(string country, string city, string street, string contactPhoneNumber, OrderStatus status, decimal totalAmount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(contactPhoneNumber);
        ValidateStatus(status);
        ArgumentOutOfRangeException.ThrowIfNegative(totalAmount);
    }

    private static void ValidateStatus(OrderStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), "Order status must be a defined value.");
        }
    }

    private static void ValidateProduct(int quantity, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
    }
}
