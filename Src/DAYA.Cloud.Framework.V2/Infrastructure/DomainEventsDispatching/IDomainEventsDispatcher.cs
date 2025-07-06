using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;

internal interface IDomainEventsDispatcher
{
    Task<IEnumerable<OutboxMessageRefrences>> DispatchEventsAsync();
}
