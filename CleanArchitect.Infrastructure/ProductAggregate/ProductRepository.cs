using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Domain.ProductAggregate;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitect.Infrastructure.ProductAggregate;

public sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Products.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Products.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken)
    {
        return await dbContext.Products.AsNoTracking()
            .Where(product => ids.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }

    public void Add(Product product)
    {
        dbContext.Products.Add(product);
    }

    public void Remove(Product product)
    {
        dbContext.Products.Remove(product);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
