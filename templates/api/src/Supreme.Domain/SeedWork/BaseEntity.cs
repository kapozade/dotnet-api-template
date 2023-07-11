using Supreme.Domain.DomainEvents;
using System.Collections.Concurrent;

namespace Supreme.Domain.SeedWork;

public abstract class BaseEntity
{
    public int CreatedBy { get; protected set; }
    public int ModifiedBy { get; protected set; }
    public DateTime CreatedOn { get; protected set; }
    public DateTime ModifiedOn { get; protected set; }
    
    private readonly ConcurrentQueue<IDomainEvent> _events = new();

    public IReadOnlyCollection<IDomainEvent> Events
    {
        get => _events;
    }

    protected BaseEntity() { }
    
    protected void SetModificationAudit(int executedBy, DateTime? executedOn = null)
    {
        ModifiedBy = executedBy;
        ModifiedOn = executedOn ?? DateTime.UtcNow;
    }

    protected void AddEvent(IDomainEvent @event)
    {
        _events.Enqueue(@event);
    }

    public bool TryRemove(out IDomainEvent? @event)
    {
        return _events.TryDequeue(out @event);
    }
}
