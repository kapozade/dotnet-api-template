using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Supreme.Domain.Constants;
using Supreme.Domain.Exceptions;
using Supreme.Domain.SeedWork;

namespace Supreme.Infrastructure.Db;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbSet<TEntity> _dbSet;
    public BaseRepository(
        SupremeContext dbContext
        )
    {
        _dbSet = dbContext.Set<TEntity>();
    }

    public virtual async Task<TEntity?> TryGetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        var entity = await TryGetByIdAsync(id, cancellationToken);
        return entity
               ?? throw new NotFoundException(MessageKeys.GenerateMessage(MessageKeys._notFound, typeof(TEntity).Name.ToLower()));
    }

    public virtual async Task<TEntity?> TryGetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TEntity> GetAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var entity = await TryGetAsync(specification, cancellationToken);
        return entity
               ?? throw new NotFoundException(MessageKeys.GenerateMessage(MessageKeys._notFound, typeof(TEntity).Name.ToLower()));
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .CountAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await SpecificationEvaluator.Default
            .GetQuery(_dbSet.AsQueryable(), specification)
            .AnyAsync(cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
