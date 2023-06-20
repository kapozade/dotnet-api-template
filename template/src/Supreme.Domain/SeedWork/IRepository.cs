namespace Supreme.Domain.SeedWork;

public interface IRepository<TEntity>: IReadRepositoryBase<TEntity> where TEntity : BaseEntity
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
}