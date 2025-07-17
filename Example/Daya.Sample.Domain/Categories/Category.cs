using Daya.Sample.Domain.Categories.Events;
using Daya.Sample.Domain.Commons;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.DirectOperations.Attributes;
using DAYA.Cloud.Framework.V2.Domain;
using Newtonsoft.Json;

namespace Daya.Sample.Domain.Categories
{
    [ContainerName(ServiceDatabaseContainers.Categories)]
    [PartitionKeyPath("partitionKey")]
    public class Category : CosmosEntity
    {
        public override CategoryId Id { get; } = null!;
        public string PartitionKey { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        [JsonConstructor]
        private Category(CategoryId id)
        {
            Id = id;
        }

        public static async Task<Category> CreateAsync(
            CategoryId id,
            string partitionKey,
            string name,
            string? description)
        {
            var @event = new CategoryCreatedDomainEvent(
                id,
                partitionKey,
                name,
                description);

            var category = new Category(id);

            category.Apply(@event);
            category.AddDomainEvent(@event);

            return await Task.FromResult(category);
        }

        public void Update(string name)
        {
            var @event = new CategoryUpdatedDomainEvent(
                Id,
                name);

            ApplyEvent(@event);
            AddDomainEvent(@event);
        }

        protected override void Apply(IDomainEvent @event)
        {
            ApplyEvent(@event as dynamic);
        }

        private void ApplyEvent(CategoryCreatedDomainEvent @event)
        {
            Name = @event.Name;
            Description = @event.Description;
            PartitionKey = @event.PartitionKey;
        }

        private void ApplyEvent(CategoryUpdatedDomainEvent @event)
        {
            Name = @event.Name;
        }
    }
}