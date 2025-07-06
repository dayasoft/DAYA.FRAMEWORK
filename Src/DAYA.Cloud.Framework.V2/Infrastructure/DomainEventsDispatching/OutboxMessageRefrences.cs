using System;

namespace DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;

internal class OutboxMessageRefrences
{
    public OutboxMessageRefrences(Guid id,
        string aggregateId,
        DateTime occurredOn,
        string type)
    {
        Id = id;
        AggregateId = aggregateId;
        OccurredOn = occurredOn;
        Type = type;
    }

    public Guid Id { get; set; }
    public string AggregateId { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Type { get; set; }
}