using System.Collections.Generic;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.DirectOperations;

public interface ICosmosEntityChangeTracker
{
    void Track(CosmosEntity entity, string containerName);

    List<CosmosEntity> GetTrackedEntities();

    List<IDomainEvent> GetDomainEvents();
}