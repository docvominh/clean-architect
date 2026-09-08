namespace CleanArchitect.Domain.OrderAggregate;

public class OrderProduct : BaseEntity
{
    public OrderProduct(Guid id, Guid createBy, Guid orderId, Guid productId, int quantity, decimal unitPrice) : base(id, createBy)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid OrderId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}
