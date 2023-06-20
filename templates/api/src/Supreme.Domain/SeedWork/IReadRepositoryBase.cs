using Ardalis.Specification;

namespace Supreme.Domain.SeedWork;

public interface IReadRepositoryBase<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> TryGetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<TEntity?> TryGetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<TEntity> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
}
