using CleanArchitect.Application.OrderAggregate;
using CleanArchitect.Domain.OrderAggregate;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitect.Infrastructure.OrderAggregate;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public Task<List<Order>> GetByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.Orders
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
