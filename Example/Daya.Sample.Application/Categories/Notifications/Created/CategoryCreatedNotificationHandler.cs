using Daya.Sample.Application.Categories.Commands.Update;
using Daya.Sample.Domain.Categories.Events;
using Daya.Sample.IntegrationEvents.Categories;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Configuration.Notifications;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;

namespace Daya.Sample.Application.Categories.Notifications.Created
{
    public class CategoryCreatedNotificationHandler : IDomainNotificationHandler<CategoryCreatedDomainEvent>
    {
        private readonly ICommandsScheduler _commandsScheduler;
        private readonly IEventBus _eventBus;

        public CategoryCreatedNotificationHandler(ICommandsScheduler commandsScheduler, IEventBus eventBus)
        {
            _commandsScheduler = commandsScheduler;
            _eventBus = eventBus;
        }

        public async Task Handle(CategoryCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await _eventBus.Publish(new CategorycreatedIntegrationEvent(
                    domainEvent.CategoryId,
                    domainEvent.Name));
        }
    }
}