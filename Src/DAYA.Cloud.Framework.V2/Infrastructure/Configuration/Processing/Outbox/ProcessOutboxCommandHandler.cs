using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Outbox;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.RetryPolicy;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox;

internal class ProcessOutboxCommandHandler : IRequestHandler<ProcessOutboxCommand>
{
    private readonly Container _outboxContainer;
    private readonly IApplicationAssemblyResolver _assemblyResolver;
    private readonly PollyConfig _pollyConfig;
    private readonly ILogger<ProcessOutboxCommandHandler> _logger;

    public ProcessOutboxCommandHandler(IContainerFactory containerFactory,
        IApplicationAssemblyResolver assemblyResolver,
        PollyConfig pollyConfig,
        ILogger<ProcessOutboxCommandHandler> logger)
    {
        _outboxContainer = containerFactory.Get(ServiceDatabaseContainersBase.OutboxMessages);
        _assemblyResolver = assemblyResolver;
        _pollyConfig = pollyConfig;
        _logger = logger;
    }

    public async Task Handle(ProcessOutboxCommand request, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(request.MessageId);

        try
        {
            var outboxMessage = await _outboxContainer.ReadItemAsync<OutboxMessage>(
                messageId.ToString(),
                new PartitionKey(messageId.ToString()),
                cancellationToken: cancellationToken);

            // process outboxMessage as before
            var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_pollyConfig.SleepDurations);
            var result = await policy.ExecuteAndCaptureAsync(() => ProcessCommandAndDeleteAsync(outboxMessage));
            if (result.Outcome == OutcomeType.Failure)
            {
                _logger.LogError("failed to process outbox message.");
                outboxMessage.Resource.Error = result.FinalException.ToString();
                outboxMessage.Resource.ProcessedDate = Clock.Now;
            }
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Outbox message not found! Skipping.");
            return;
        }
    }

    private async Task ProcessCommandAndDeleteAsync(OutboxMessage outboxMessage)
    {
        var type = _assemblyResolver.Resolve().GetType(outboxMessage.Type);
        var commandToProcess = JsonConvert.DeserializeObject(outboxMessage.Data, type) as dynamic;
        await CommandsExecutor.ExecuteAndForget(commandToProcess);

        //using var scope = ServiceCompositionRoot.CreateScope();
        //var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        //await mediator.Send(commandToProcess);

        await _outboxContainer.DeleteItemAsync<OutboxMessage>(
            outboxMessage.Id.ToString(),
            new PartitionKey(outboxMessage.Id.ToString()));
    }
}