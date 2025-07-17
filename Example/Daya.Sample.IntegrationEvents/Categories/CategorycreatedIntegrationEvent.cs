using Daya.Sample.Domain.Categories;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;

namespace Daya.Sample.IntegrationEvents.Categories
{
    public record CategorycreatedIntegrationEvent(
    CategoryId CategoryId,
    string Name) : IntegrationEvent(CategoryId, "CategoryCreatedEvent");
}