using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.Categories.Events
{
    public record CategoryUpdatedDomainEvent(
        CategoryId CategoryId,
        string Name,
        CategoryTag Tag) : DomainEventBase(CategoryId);
}