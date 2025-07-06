using Azure.Messaging.ServiceBus;

namespace DAYA.Cloud.Framework.V2.ServiceBus;

public interface ITopicClientFactory
{
    ServiceBusSender CreateSender(string topic);
}