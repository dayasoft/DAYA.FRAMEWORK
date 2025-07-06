using System.Collections.Generic;

namespace DAYA.Cloud.Framework.V2.Domain;

public abstract class AggregateRoot : Entity
{
    private List<IDomainEvent> _domainEvents;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    protected abstract void Apply(IDomainEvent @event);

    public void Load(IDomainEvent @event)
    {
        Apply(@event);
    }
}
