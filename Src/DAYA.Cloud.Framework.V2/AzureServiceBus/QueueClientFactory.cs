using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;

namespace DAYA.Cloud.Framework.V2.AzureServiceBus;

internal class QueueClientFactory : IQueueClientFactory
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _cache = new();

    public QueueClientFactory(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public ServiceBusSender CreateSender(string queueName)
    {
        return _cache.GetOrAdd(queueName, _serviceBusClient.CreateSender(queueName));
    }
}