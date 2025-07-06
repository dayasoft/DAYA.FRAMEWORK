using Azure.Messaging.ServiceBus;

namespace DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;

public interface IQueueClientFactory
{
    ServiceBusSender CreateSender(string queueName);
}