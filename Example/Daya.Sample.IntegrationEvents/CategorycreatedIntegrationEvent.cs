using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;

namespace Daya.Sample.IntegrationEvents
{
    public record CategorycreatedIntegrationEvent(
    CategoryId CategoryId,
    string Name) : IntegrationEvent(CategoryId, "CategoryCreatedEvent");
}