using System;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Infrastructure.EventBus;

public interface IEventBus : IDisposable
{
    Task Publish<T>(T @event)
        where T : IntegrationEvent;
}
