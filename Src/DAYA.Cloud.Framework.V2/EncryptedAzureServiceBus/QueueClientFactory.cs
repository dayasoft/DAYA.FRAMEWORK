using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using System.Collections.Concurrent;

namespace DAYA.Cloud.Framework.V2.EncryptedAzureServiceBus;

internal class QueueClientFactory : IQueueClientFactory
{
    private readonly ServiceBusConfig _serviceBusConfig;

    private readonly ConcurrentDictionary<string, ServiceBusSender> _cache = new();

    public QueueClientFactory(ServiceBusConfig serviceBusConfig)
    {
        _serviceBusConfig = serviceBusConfig;
    }

    public ServiceBusSender CreateSender(string queueName)
    {
        return _cache.GetOrAdd(queueName, new ServiceBusClient(_serviceBusConfig.ConnectionString).CreateSender(queueName));
    }
}