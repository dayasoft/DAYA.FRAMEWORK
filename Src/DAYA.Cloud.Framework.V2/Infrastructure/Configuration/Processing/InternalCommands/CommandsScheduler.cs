using System;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Common.Constants;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;

internal class CommandsScheduler : ICommandsScheduler
{
    private readonly ILogger<CommandsScheduler> _logger;
    private readonly Container _internalCommandContainer;
    private readonly IQueueMessagePublisher _queueMessagePublisher;
    private readonly InternalCommandConfig _internalCommandConfig;

    public CommandsScheduler(
        ILogger<CommandsScheduler> logger,
        IContainerFactory containerFactory,
        IQueueMessagePublisher queueMessagePublisher,
        InternalCommandConfig internalCommandConfig)
    {
        _logger = logger;
        _internalCommandContainer = containerFactory.Get(ServiceDatabaseContainersBase.InternalCommands);
        _queueMessagePublisher = queueMessagePublisher;
        _internalCommandConfig = internalCommandConfig;
    }

    public async Task EnqueueAsync(IDirectCommand command)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResullt>(IUpdateSearchCommand<TResullt> command)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResullt>(IRemoveSearchCommand<TResullt> command)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResult>(IDirectCommand<TResult> command)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync(IDirectCommand command, string? sessionId)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow,
            sessionId);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResullt>(IUpdateSearchCommand<TResullt> command, string? sessionId)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow,
            sessionId);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResullt>(IRemoveSearchCommand<TResullt> command, string? sessionId)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow,
            sessionId);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    public async Task EnqueueAsync<TResult>(IDirectCommand<TResult> command, string? sessionId)
    {
        var internalCommand = new InternalCommandMessage(
            command.InternalProcessId,
            command.GetType().FullName,
            JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            }),
            DateTime.UtcNow,
            sessionId);

        var json = JsonConvert.SerializeObject(internalCommand, Formatting.Indented);
        _logger.LogInformation("Enqueue Command:");
        _logger.LogInformation(json);

        await AddAsync(internalCommand);
    }

    private async Task AddAsync(InternalCommandMessage internalCommand)
    {
        await _internalCommandContainer.CreateItemAsync(internalCommand, new PartitionKey(internalCommand.Id.ToString()));
        await PublishInternalCommandMessage(internalCommand);
    }

    private async Task PublishInternalCommandMessage(InternalCommandMessage internalCommand)
    {
        if (internalCommand is null)
        {
            return;
        }

        _logger.LogInformation($"Sending Internal command message to internalCommands queue");
        var rawData = JsonConvert.SerializeObject(internalCommand);
        await _queueMessagePublisher.PublishAsync(
                _internalCommandConfig.QueueName,
                internalCommand.SessionId ?? internalCommand.Type,
                rawData);

        _logger.LogInformation("All messages have been sent to internalCommands queue");
    }
}