using System;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static DAYA.Cloud.Framework.V2.DirectOperations.DirectUnitOfWork;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox
{
    public class OutboxMessageProcessor : IOutboxMessageProcessor
    {
        private readonly IOutboxMessageRepository _outboxRepository;
        private readonly ILogger<OutboxMessageProcessor> _logger;
        private readonly IApplicationAssemblyResolver _assemblyResolver;

        public OutboxMessageProcessor(IOutboxMessageRepository outboxRepository,
            ILogger<OutboxMessageProcessor> logger,
            IApplicationAssemblyResolver assemblyResolver)
        {
            _outboxRepository = outboxRepository;
            _logger = logger;
            _assemblyResolver = assemblyResolver;
        }

        public async Task ProcessMessageAsync(string messageBody)
        {
            try
            {
                // Deserialize the reference message
                var reference = JsonConvert.DeserializeObject<OutboxMessageReference>(messageBody);

                // Get the outbox message from Cosmos DB
                var outboxMessage = await _outboxRepository.GetByIdAsync(reference.Id.ToString(), reference.Id.ToString());

                if (outboxMessage == null)
                {
                    _logger.LogWarning("Outbox message not found: {MessageId}", reference.Id);
                    return;
                }

                if (outboxMessage.ProcessedDate is not null)
                {
                    _logger.LogInformation("Message already processed: {MessageId}", reference.Id);
                    return;
                }

                // Deserialize and execute the notification
                await ExecuteDomainEventNotificationAsync(outboxMessage);

                _logger.LogInformation("Successfully processed outbox message: {MessageId}", outboxMessage.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message: {MessageBody}", messageBody);
            }
        }

        private async Task ExecuteDomainEventNotificationAsync(OutboxMessage outboxMessage)
        {
            var outboxMessageDataType = Type.GetType(outboxMessage.Type);
            if (outboxMessageDataType == null)
            {
                throw new InvalidOperationException($"Could not resolve message type: {outboxMessage.Type}");
            }

            var domainEventNotification = JsonConvert.DeserializeObject(outboxMessage.Data, outboxMessageDataType) as dynamic;

            _ = Task.Run(async () =>
            {
                // this scope will live for the lifetime of the Send(...)
                using var scope = ServiceCompositionRoot.ServiceProvider.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                try
                {
                    await mediator.Publish((dynamic)domainEventNotification);
                    await _outboxRepository.DeleteAsync(outboxMessage.Id.ToString(), outboxMessage.Id.ToString());
                    Console.WriteLine($"Notification fired!");
                }
                catch (Exception ex)
                {
                    outboxMessage.ProcessedDate = DateTime.UtcNow;
                    outboxMessage.Error = ex.ToString();
                    await _outboxRepository.UpdateAsync(outboxMessage);
                    Console.Error.WriteLine($"Fire-and-forget notification failed: {ex}");
                }
            });

            await Task.CompletedTask; // Placeholder for the actual send operation
        }
    }
}