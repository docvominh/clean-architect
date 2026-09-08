namespace CleanArchitect.Domain.Order;

public class OrderProduct : BaseEntity
{
    public required Guid OrderId { get; set; }

    public required Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
