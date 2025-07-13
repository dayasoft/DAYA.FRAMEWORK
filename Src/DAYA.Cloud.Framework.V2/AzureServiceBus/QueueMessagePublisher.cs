using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.AzureServiceBus;

internal class QueueMessagePublisher : IQueueMessagePublisher
{
    private readonly IQueueClientFactory _queueClientFactory;
    private readonly ILogger<QueueMessagePublisher> _logger;
    private readonly ServiceBusQueuePublisherCompressionOptions _compressionOptions;

    public QueueMessagePublisher(
        IQueueClientFactory queueClient,
        ILogger<QueueMessagePublisher> logger,
        ServiceBusQueuePublisherCompressionOptions compressionOptions)
    {
        _queueClientFactory = queueClient;
        _logger = logger;
        _compressionOptions = compressionOptions;
    }

    public async Task PublishAsync(string queue, string sessionId, string raw)
    {
        _logger.LogInformation($"Publishing message to queue {queue}");

        // Convert to bytes
        byte[] data = Encoding.UTF8.GetBytes(raw);
        var contentType = AzureServiceBusConstants.MessageContentTypes.ApplicationJson;

        // Optional compression
        if (_compressionOptions.EnableCompression)
        {
            data = data.CompressData();
            contentType = AzureServiceBusConstants.MessageContentTypes.Gzip;
        }

        var message = new ServiceBusMessage(data)
        {
            SessionId = sessionId,
            PartitionKey = sessionId,
            ContentType = contentType
        };

        var sender = _queueClientFactory.CreateSender(queue);

        await sender.SendMessageAsync(message);
    }

    public Task PublishAsync<T>(string queue, string sessionId, T obj)
    {
        return PublishAsync(queue, sessionId, JsonConvert.SerializeObject(obj, Formatting.Indented));
    }
}