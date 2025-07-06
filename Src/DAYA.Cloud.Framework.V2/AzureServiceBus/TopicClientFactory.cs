using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.ServiceBus;

namespace DAYA.Cloud.Framework.V2.AzureServiceBus;

internal class TopicClientFactory : ITopicClientFactory
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ConcurrentDictionary<string, ServiceBusSender> _clients = new();

    public TopicClientFactory(ServiceBusClient serviceBusClient)
    {
        _serviceBusClient = serviceBusClient;
    }

    public ServiceBusSender CreateSender(string topic) =>
        _clients.GetOrAdd(topic, t => _serviceBusClient.CreateSender(topic));
}