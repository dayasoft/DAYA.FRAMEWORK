using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Application.Events;

public interface IDomainEventNotification<out TEvent> : IDomainNotificationRequest
    where TEvent : IDomainEvent
{
    TEvent DomainEvent { get; }
}