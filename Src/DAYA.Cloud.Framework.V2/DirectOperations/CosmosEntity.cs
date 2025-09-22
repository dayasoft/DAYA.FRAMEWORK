using System.Collections.Generic;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Domain;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.DirectOperations;

public abstract class CosmosEntity : Entity
{
    [JsonIgnore]
    public abstract TypedId Id { get; }

    [JsonProperty("_etag")]
    public string ETag { get; set; }

    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = new();

    [JsonIgnore]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected abstract void Apply(IDomainEvent @event);
}