using System;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands
{
    public class InternalMessageProcessor : IInternalMessageProcessor
    {
        private readonly IInternalMessageRepository _internalMessageRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<InternalMessageProcessor> _logger;
        private readonly IApplicationAssemblyResolver _assemblyResolver;

        public InternalMessageProcessor(IInternalMessageRepository internalMessageRepository,
            IMediator mediator,
            ILogger<InternalMessageProcessor> logger,
            IApplicationAssemblyResolver assemblyResolver)
        {
            _internalMessageRepository = internalMessageRepository;
            _mediator = mediator;
            _logger = logger;
            _assemblyResolver = assemblyResolver;
        }

        public async Task ProcessMessageAsync(string messageBody)
        {
            try
            {
                // Deserialize the reference message
                var reference = JsonConvert.DeserializeObject<InternalCommandMessage>(messageBody);

                // Get the outbox message from Cosmos DB
                var internalMessage = await _internalMessageRepository.GetByIdAsync(reference.Id.ToString(), reference.Id.ToString());

                if (internalMessage == null)
                {
                    _logger.LogWarning("Internal command message not found: {MessageId}", reference.Id);
                    return;
                }

                if (internalMessage.ProcessedDate is not null)
                {
                    _logger.LogInformation("Message already processed: {MessageId}", reference.Id);
                    return;
                }

                // Deserialize and execute the notification
                await ExecuteInternalCommandMessageAsync(internalMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message: {MessageBody}", messageBody);
            }
        }

        private async Task ExecuteInternalCommandMessageAsync(InternalCommandMessage internalMessage)
        {
            Type type = _assemblyResolver.Resolve().GetType(internalMessage.Type);
            dynamic commandToProcess = JsonConvert.DeserializeObject(internalMessage.Data, type);

            try
            {
                await CommandsExecutor.Execute(commandToProcess);
                await _internalMessageRepository.DeleteAsync(internalMessage.Id.ToString(), internalMessage.Id.ToString());
            }
            catch (Exception ex)
            {
                // TODO: log the exception somewhere you can observe it
                Console.Error.WriteLine($"Fire-and-forget notification failed: {ex}");
            }
        }
    }
}