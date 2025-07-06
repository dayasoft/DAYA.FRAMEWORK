using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Application.InternalCommands
{
    public class InternalCommandMessageBackgroundService : BackgroundService
    {
        private readonly ServiceBusSessionProcessor _serviceBusProcessor;
        private readonly IInternalMessageProcessor _internalMessageProcessor;
        private readonly ILogger<InternalCommandMessageBackgroundService> _logger;

        public InternalCommandMessageBackgroundService(
            ILogger<InternalCommandMessageBackgroundService> logger,
            ServiceBusClient serviceBusClient,
            IInternalMessageProcessor internalMessageProcessor)
        {
            _logger = logger;
            _serviceBusProcessor = serviceBusClient.CreateSessionProcessor("internalcommandmessage", new ServiceBusSessionProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentSessions = 10, // Number of concurrent sessions to process
                MaxConcurrentCallsPerSession = 5, // Messages processed concurrently per session
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5),
                PrefetchCount = 50, // Number of messages to prefetch
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });

            _internalMessageProcessor = internalMessageProcessor;

            _serviceBusProcessor.ProcessMessageAsync += ProcessInternalMessageAsync;
            _serviceBusProcessor.ProcessErrorAsync += ProcessInternalMessageErrorAsync;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _serviceBusProcessor.StartProcessingAsync(stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            finally
            {
                await _serviceBusProcessor.StopProcessingAsync(CancellationToken.None);
            }

            await _serviceBusProcessor.StopProcessingAsync(stoppingToken);
            await Task.CompletedTask;
        }

        private async Task ProcessInternalMessageAsync(ProcessSessionMessageEventArgs args)
        {
            try
            {
                var messageBody = args.Message.Body.ToString();
                await _internalMessageProcessor.ProcessMessageAsync(messageBody);

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

        private Task ProcessInternalMessageErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus processing error: {ErrorSource}", args.ErrorSource);
            return Task.CompletedTask;
        }

        //public override void Dispose()
        //{
        //    _processor?.DisposeAsync().GetAwaiter().GetResult();
        //    base.Dispose();
        //}
    }
}