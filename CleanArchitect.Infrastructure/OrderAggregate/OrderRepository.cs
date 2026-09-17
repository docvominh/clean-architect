using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Domain.OrderAggregate;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitect.Infrastructure.OrderAggregate;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task<Order?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderProducts)
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Order>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .Include(o => o.OrderProducts)
            .Where(o => o.CreateBy == userId)
            .OrderByDescending(o => o.CreatedAt)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public void Add(Order order)
    {
        dbContext.Orders.Add(order);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
