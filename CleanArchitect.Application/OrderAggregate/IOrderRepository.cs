using CleanArchitect.Domain.OrderAggregate;

namespace CleanArchitect.Application.OrderAggregate;

public interface IOrderRepository
{
    Task<Order?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Order>> GetByUserAsync(Guid userId, CancellationToken cancellationToken);

    void Add(Order order);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
