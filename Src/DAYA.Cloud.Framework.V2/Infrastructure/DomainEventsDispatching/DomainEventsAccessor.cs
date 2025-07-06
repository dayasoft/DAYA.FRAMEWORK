using System.Collections.Generic;
using System.Linq;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;

internal class DomainEventsAccessor : IDomainEventsAccessor
{
    private readonly ServiceDbContext _dbContext;

    public DomainEventsAccessor(ServiceDbContext meetingsContext)
    {
        _dbContext = meetingsContext;
    }

    public List<IDomainEvent> GetAllDomainEvents()
    {
        var domainEntities = _dbContext
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        return domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();
    }

    public void ClearAllDomainEvents()
    {
        var domainEntities = _dbContext
            .ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());
    }
}
