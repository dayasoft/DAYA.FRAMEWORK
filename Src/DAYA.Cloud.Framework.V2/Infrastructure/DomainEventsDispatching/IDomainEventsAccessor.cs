using System.Collections.Generic;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;

internal interface IDomainEventsAccessor
{
    List<IDomainEvent> GetAllDomainEvents();

    void ClearAllDomainEvents();
}
