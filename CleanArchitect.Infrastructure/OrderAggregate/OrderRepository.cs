using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Infrastructure.OrderAggregate;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public void Add(Order order)
    {
        dbContext.Orders.Add(order);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
