using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.ServiceBus;
using System.Collections.Concurrent;

namespace DAYA.Cloud.Framework.V2.EncryptedAzureServiceBus;

internal class TopicClientFactory : ITopicClientFactory
{
    private readonly ServiceBusConfig _serviceBusConfig;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _clients = new();

    public TopicClientFactory(ServiceBusConfig serviceBusConfig)
    {
        _serviceBusConfig = serviceBusConfig;
    }

    public ServiceBusSender CreateSender(string topic) =>
        _clients.GetOrAdd(topic, t => new ServiceBusClient(_serviceBusConfig.ConnectionString).CreateSender(topic));
}