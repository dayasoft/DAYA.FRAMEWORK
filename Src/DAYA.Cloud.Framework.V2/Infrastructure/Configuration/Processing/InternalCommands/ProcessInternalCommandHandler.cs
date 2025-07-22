using System;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Cosmos;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.RetryPolicy;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;

internal class ProcessInternalCommandHandler : ICommandHandler<ProcessInternalCommand>
{
    private readonly Container _internalContainer;
    private readonly IApplicationAssemblyResolver _assemblyResolver;
    private readonly PollyConfig _pollyConfig;
    private readonly ILogger _logger;

    public ProcessInternalCommandHandler(
        ContainerFactory containerFactory,
        IApplicationAssemblyResolver assemblyResolver,
        PollyConfig pollyConfig,
        ILogger logger)
    {
        _internalContainer = containerFactory.Get(ServiceDatabaseContainersBase.InternalCommands);
        _assemblyResolver = assemblyResolver;
        _pollyConfig = pollyConfig;
        _logger = logger;
    }

    public async Task Handle(ProcessInternalCommand request, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(request.MessageId);

        try
        {
            var internalCommandMessage = await _internalContainer.ReadItemAsync<InternalCommandMessage>(messageId.ToString(),
                new PartitionKey(messageId.ToString()));

            var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_pollyConfig.SleepDurations);
            var result = await policy.ExecuteAndCaptureAsync(() => ProcessAndDeleteCommandAsync(internalCommandMessage, cancellationToken));

            if (result.Outcome == OutcomeType.Failure)
            {
                _logger.LogError("failed to process internal command.");
                internalCommandMessage.Resource.Error = result.FinalException.ToString();
                internalCommandMessage.Resource.ProcessedDate = Clock.Now;
            }
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning("internal command not found! skipping.");
            return;
        }
    }

    private async Task ProcessAndDeleteCommandAsync(InternalCommandMessage internalCommand, CancellationToken cancellationToken)
    {
        Type type = _assemblyResolver.Resolve().GetType(internalCommand.Type);
        dynamic commandToProcess = JsonConvert.DeserializeObject(internalCommand.Data, type);

        await CommandsExecutor.Execute(commandToProcess);
        await _internalContainer.DeleteItemAsync<InternalCommandMessage>(internalCommand.Id.ToString(),
            new PartitionKey(internalCommand.Id.ToString()), cancellationToken: cancellationToken);
    }
}