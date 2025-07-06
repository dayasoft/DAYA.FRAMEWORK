using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Events;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;

internal class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IDomainEventsAccessor _domainEventsProvider;
    private readonly IOutboxMessageRepository _outbox;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventsDispatcher(IDomainEventsAccessor domainEventsProvider,
        IOutboxMessageRepository outbox,
        IServiceProvider serviceProvider)
    {
        _domainEventsProvider = domainEventsProvider;
        _outbox = outbox;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<OutboxMessageRefrences>> DispatchEventsAsync()
    {
        var domainEvents = _domainEventsProvider.GetAllDomainEvents();

        var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();

        foreach (var domainEvent in domainEvents)
        {
            Type domainEventNotificationType = typeof(IDomainEventNotification<>);
            var domainNotificationWithGenericType = domainEventNotificationType.MakeGenericType(domainEvent.GetType());

            // Create a scope and try to resolve the service
            object domainNotification = null;
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Try to create instance with parameters
                    domainNotification = ActivatorUtilities.CreateInstance(
                        scope.ServiceProvider,
                        domainNotificationWithGenericType,
                        new object[] { domainEvent, Guid.NewGuid() });
                }
            }
            catch (InvalidOperationException)
            {
                // Equivalent to ResolveOptional returning null - service not found
            }

            if (domainNotification != null)
            {
                domainEventNotifications.Add(domainNotification as IDomainEventNotification<IDomainEvent>);
            }
        }

        _domainEventsProvider.ClearAllDomainEvents();

        var outboxIds = new List<OutboxMessageRefrences>();
        foreach (var domainEventNotification in domainEventNotifications)
        {
            var type = domainEventNotification.GetType().FullName;
            var data = JsonConvert.SerializeObject(domainEventNotification, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            });
            var outboxMessage = new OutboxMessage(
                domainEventNotification.DomainEvent.OccurredOn,
                type,
                data);
            outboxIds.Add(new OutboxMessageRefrences(
                outboxMessage.Id,
                domainEventNotification.DomainEvent.AggregateId,
                outboxMessage.OccurredOn,
                outboxMessage.Type));
            await _outbox.CreateAsync(outboxMessage);
        }
        return outboxIds;
    }
}