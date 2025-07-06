using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Application.Outbox
{
    public class OutboxMessageBackgroundService : BackgroundService
    {
        private readonly ServiceBusSessionProcessor _serviceBusSessionProcessor;
        private readonly IOutboxMessageProcessor _outboxMessageProcessor;
        private readonly ILogger<OutboxMessageBackgroundService> _logger;

        public OutboxMessageBackgroundService(
            ILogger<OutboxMessageBackgroundService> logger,
            ServiceBusClient serviceBusClient,
            IOutboxMessageProcessor outboxMessageProcessor)
        {
            _logger = logger;
            _outboxMessageProcessor = outboxMessageProcessor;

            _serviceBusSessionProcessor = serviceBusClient.CreateSessionProcessor("outboxMessages", new ServiceBusSessionProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentSessions = 10, // Number of concurrent sessions to process
                MaxConcurrentCallsPerSession = 5, // Messages processed concurrently per session
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                PrefetchCount = 50 // Number of messages to prefetch
            });

            _serviceBusSessionProcessor.ProcessMessageAsync += ProcessOutboxMessageAsync;
            _serviceBusSessionProcessor.ProcessErrorAsync += ProcessOutboxErrorAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serviceBusSessionProcessor.StartProcessingAsync(stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            finally
            {
                await _serviceBusSessionProcessor.StopProcessingAsync(CancellationToken.None);
            }

            await _serviceBusSessionProcessor.StopProcessingAsync(stoppingToken);
            await Task.CompletedTask;
        }

        private async Task ProcessOutboxMessageAsync(ProcessSessionMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                await _outboxMessageProcessor.ProcessMessageAsync(messageBody);

                // Complete the message
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Service Bus message: {MessageId}", args.Message.MessageId);

                // move the message to deadletter
                await args.DeadLetterMessageAsync(args.Message);
            }
        }

        private Task ProcessOutboxErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus processing error: {ErrorSource}", args.ErrorSource);
            return Task.CompletedTask;
        }
    }
}