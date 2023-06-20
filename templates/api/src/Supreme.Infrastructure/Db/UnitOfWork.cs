using Supreme.Domain.Entities.Foo;
using Supreme.Domain.Db;
using Supreme.Domain.Enums;
using Supreme.Infrastructure.Db.Extensions;
using Supreme.Infrastructure.Db.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Supreme.Domain.SeedWork;
using System.Data;

namespace Supreme.Infrastructure.Db;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SupremeContext _context;
    private readonly IMediator _mediator;
    private IFooRepository? _fooRepository;
    private bool _disposed;

    public UnitOfWork(
        SupremeContext context,
        IMediator mediator
        )
    {
        _context = context;
        _mediator = mediator;
    }
    
    public IFooRepository FooRepository
    {
        get => _fooRepository ??= new FooRepository(_context);
    }

    public IDbTransaction BeginTransaction(TransactionLevels? transactionLevels = null)
    {
        var isolationLevel = transactionLevels.ToIsolationLevel();
        return isolationLevel == null
            ? _context.Database.BeginTransaction().GetDbTransaction()
            : _context.Database.BeginTransaction(isolationLevel.Value).GetDbTransaction();
    }

    public async Task CommitChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
        await PublishDomainEventAsync(cancellationToken);
    }

    public async Task CheckHealthAsync()
    {
        const string sqlToExecute = "SELECT 1;";
        await _context.Database.ExecuteSqlRawAsync(sqlToExecute);
    }
    
    public void Dispose()
    {
        Dispose(true); 
        GC.SuppressFinalize(this);
    }

    private async Task PublishDomainEventAsync(CancellationToken cancellationToken)
    {
        bool again;
        do
        {
            var aggregates = _context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is BaseEntity baseAggregate && baseAggregate.Events.Any())
                .Select(x => (BaseEntity)x.Entity)
                .ToList();

            again = aggregates.Any();

            foreach (var aggregate in aggregates)
                if (aggregate.TryRemove(out var domainEvent))
                    if (domainEvent != null) 
                        await _mediator.Publish(domainEvent!, cancellationToken);

        } while (again);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing) _context.Dispose();

        _disposed = true;
    }
}
