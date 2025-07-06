using Azure.Messaging.ServiceBus;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using DAYA.Cloud.Framework.V2.ServiceBus;
using DAYA.Cloud.Framework.V2.SymmetricEncryption;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.EncryptedAzureServiceBus;

internal class AzureServiceBusEventBus : IEventBus
{
    private readonly ITopicClientFactory _topicClientFactory;
    private readonly ILogger _logger;
    private readonly ISymmetricEncryption _symmetricEncryption;
    private readonly ServiceBusQueuePublisherCompressionOptions _compressionOptions;

    public AzureServiceBusEventBus(
        ITopicClientFactory topicClientFactory,
        ISymmetricEncryption symmetricEncryption,
        ILogger logger, ServiceBusQueuePublisherCompressionOptions compressionOptions)
    {
        _topicClientFactory = topicClientFactory;
        _symmetricEncryption = symmetricEncryption;
        _logger = logger;
        _compressionOptions = compressionOptions;
    }

    public void Dispose()
    {
    }

    public async Task Publish<T>(T @event) where T : IntegrationEvent
    {
        var eventType = @event.GetType();
        _logger.LogInformation($"Publishing {eventType.FullName}...");

        var json = JsonConvert.SerializeObject(@event, Formatting.Indented);
        var messageBody = Encoding.UTF8.GetBytes(json);
        var contentType = AzureServiceBusConstants.MessageContentTypes.MultipartEncrypted;

        if (_compressionOptions.EnableCompression)
        {
            messageBody = messageBody.CompressData();
            contentType = AzureServiceBusConstants.MessageContentTypes.GzipEncrypted;
        }

        messageBody = Encoding.UTF8.GetBytes(_symmetricEncryption.Encrypt(messageBody));

        var message = new ServiceBusMessage(messageBody)
        {
            MessageId = Guid.NewGuid().ToString(),
            SessionId = @event.AggregateId.ToString(),
            ContentType = contentType
        };
        _logger.LogInformation("Body: " + json);
        _logger.LogInformation("MessageId: " + message.MessageId);
        _logger.LogInformation("SessionId: " + message.SessionId);

        var sender = _topicClientFactory.CreateSender(@event.IntegrationEventName);
        await sender.SendMessageAsync(message);
    }
}