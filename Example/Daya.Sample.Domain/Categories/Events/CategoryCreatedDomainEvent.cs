using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.Categories.Events
{
    public record CategoryCreatedDomainEvent(
        CategoryId CategoryId,
        string PartitionKey,
        string Name,
        string? Description) : DomainEventBase(CategoryId);
}