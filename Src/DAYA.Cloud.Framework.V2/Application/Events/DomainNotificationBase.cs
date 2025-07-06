using System;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Application.Events;

public class DomainNotificationBase<T> : IDomainEventNotification<T> where T : IDomainEvent
{
    public T DomainEvent { get; }

    public Guid Id { get; }

    public DomainNotificationBase(T domainEvent)
    {
        this.Id = Guid.NewGuid();
        this.DomainEvent = domainEvent;
    }
}