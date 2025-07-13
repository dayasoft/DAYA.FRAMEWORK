using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
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
            IInternalMessageProcessor internalMessageProcessor,
            IConfiguration configuration)
        {
            _logger = logger;
            var serviceName = configuration.GetValue<string>("ServiceName").ToLower();
            _serviceBusProcessor = serviceBusClient.CreateSessionProcessor($"{serviceName}-internalcommandmessage", new ServiceBusSessionProcessorOptions
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
            try
            {
                _logger.LogInformation("Starting to process messages from the internal command queue");
                await _serviceBusProcessor.StartProcessingAsync(stoppingToken);

                // Wait until the cancellation token signals shutdown
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                // This is expected when stoppingToken is canceled, no need to handle
                _logger.LogInformation("Processing was canceled as expected during shutdown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ExecuteAsync");
            }
            finally
            {
                // Make sure we stop processing on shutdown
                await SafeStopProcessingAsync();
            }
        }

        private async Task SafeStopProcessingAsync()
        {
            try
            {
                _logger.LogInformation("Stopping the processor...");
                await _serviceBusProcessor.StopProcessingAsync();
                _logger.LogInformation("Processor stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping the processor");
            }
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping background service...");
            await SafeStopProcessingAsync();
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _serviceBusProcessor.DisposeAsync().AsTask().GetAwaiter().GetResult();
            base.Dispose();
        }
    }
}