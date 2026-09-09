using CleanArchitect.Application.OrderAggregate.Query;
using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.OrderAggregate;

using MediatR;

namespace CleanArchitect.Application.OrderAggregate.Command;

public sealed class CreateOrderCommandHandler(IOrderRepository orders, IProductRepository products) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(
            Guid.NewGuid(),
            request.CreatedBy,
            request.Request.Country,
            request.Request.City,
            request.Request.Street,
            request.Request.ContactPhoneNumber,
            request.Request.State);
        var lines = new List<OrderProductDto>();

        foreach (var item in request.Request.Items)
        {
            var product = await products.FindAsync(item.ProductId, cancellationToken)
                ?? throw new NotFoundException($"Product '{item.ProductId}' was not found.");

            var unitPrice = product.PriceDiscount ?? product.Price;
            order.AddProduct(product.Id, item.Quantity, unitPrice);
            lines.Add(new OrderProductDto(product.Id, product.Name, item.Quantity, unitPrice));
        }

        orders.Add(order);
        await orders.SaveChangesAsync(cancellationToken);

        return new OrderDto(order.Id, order.Status, order.TotalAmount, order.CreatedAt, lines);
    }
}
