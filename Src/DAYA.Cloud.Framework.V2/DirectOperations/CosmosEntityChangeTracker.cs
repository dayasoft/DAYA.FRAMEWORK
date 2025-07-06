using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.DirectOperations;

internal class CosmosEntityChangeTracker : ICosmosEntityChangeTracker
{
    private readonly Dictionary<string, CosmosEntity> _trackedEntities = new Dictionary<string, CosmosEntity>();

    public void Track(CosmosEntity entity, string containerName)
    {
        // create a unique key for the entity combined with the container name in order to track two
        // entities with the same id but in different containers at the same time
        var key = GetKey(containerName, entity.Id);
        _trackedEntities.TryAdd(key, entity);
    }

    public List<CosmosEntity> GetTrackedEntities()
    {
        return _trackedEntities.Select(x => x.Value).ToList();
    }

    private string GetKey(string containerName, TypedId entityId)
    {
        return $"{containerName}_{entityId}";
    }

    public List<IDomainEvent> GetDomainEvents()
    {
        return [.. _trackedEntities.SelectMany(x => x.Value.DomainEvents)];
    }
}