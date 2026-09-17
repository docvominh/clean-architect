using CleanArchitect.Domain.ProductAggregate;

namespace CleanArchitect.Application.ProductAggregate;

public interface IProductRepository
{
    Task<Product?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Product>> GetByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken);

    void Add(Product product);

    void Remove(Product product);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
