using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public interface IProductRepository
{
    Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);

    void Add(Product product);

    void Remove(Product product);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
