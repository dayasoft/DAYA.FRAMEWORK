using System.Reflection;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Helpers
{
    public static class OutboxMessageFactory
    {
        public static OutboxMessage CreateFrom<TEvent>(TEvent domainEvent, Assembly applicationServiceAssembly)
            where TEvent : IDomainEvent
        {
            var eventType = domainEvent.GetType();

            //// 1) Build the generic IDomainEventNotification<> interface for this event
            //var notifInterface = typeof(DomainNotificationBase<>)
            //    .MakeGenericType(eventType);

            //// 2) Scan the event's assembly for a concrete class implementing that interface
            //var notifType = applicationServiceAssembly
            //    .GetTypes()
            //    .FirstOrDefault(t =>
            //        notifInterface.IsAssignableFrom(t) &&
            //        !t.IsInterface &&
            //        !t.IsAbstract
            //    )
            //    ?? throw new InvalidOperationException(
            //        $"No IDomainEventNotification<{eventType.Name}> implementation found."
            //    );

            //var notification = (IDomainEventNotification<IDomainEvent>)
            //    Activator.CreateInstance(
            //    notifType,
            //    new object[] { domainEvent });

            // 3) Take its Assembly-Qualified Name so we can round-trip via Type.GetType later
            var notificationTypeName = eventType.AssemblyQualifiedName!;

            // 4) Serialize just the event payload
            var payload = JsonConvert.SerializeObject(domainEvent, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            });

            // 5) Build the Outbox record
            return new OutboxMessage(
                domainEvent.OccurredOn,
                notificationTypeName,
                payload);
        }
    }
}