using System;

namespace DAYA.Cloud.Framework.V2.Domain;

public record DomainEventBase : IDomainEvent
{
    public DateTime OccurredOn { get; }
    public string AggregateId { get; }

    private DomainEventBase(string aggregateId)
    {
        AggregateId = aggregateId;
        OccurredOn = Clock.Now;
    }

    public DomainEventBase(TypedId aggregateId)
        : this(aggregateId.ToString())
    {
    }
}
