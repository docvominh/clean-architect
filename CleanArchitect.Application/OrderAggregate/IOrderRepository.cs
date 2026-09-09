using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

public interface IOrderRepository
{
    void Add(Order order);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
