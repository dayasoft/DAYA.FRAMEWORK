using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Helpers;
using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.DirectOperations;

internal partial class DirectUnitOfWork : IDirectUnitOfWork
{
    private readonly IQueueMessagePublisher _messagePublisher;

    private readonly OutboxConfig _outboxConfig;

    private readonly ILogger<DirectUnitOfWork> _logger;
    private readonly ICosmosEntityChangeTracker _cosmosEntityChangeTracker;
    private readonly IServiceProvider _serviceProvider;
    private readonly IOutboxMessageRepository _outboxRepository;
    private readonly IApplicationAssemblyResolver _assemblyResolver;

    private bool _commited; public DirectUnitOfWork(
        IQueueMessagePublisher messagePublisher,
        OutboxConfig outboxConfig,
        ILogger<DirectUnitOfWork> logger,
        ICosmosEntityChangeTracker cosmosEntityChangeTracker,
        IServiceProvider serviceProvider,
        IOutboxMessageRepository outboxRepository,
        IContainerFactory containerFactory,
        IApplicationAssemblyResolver assemblyResolver)
    {
        _messagePublisher = messagePublisher;
        _outboxConfig = outboxConfig;

        _logger = logger;
        _cosmosEntityChangeTracker = cosmosEntityChangeTracker;
        _serviceProvider = serviceProvider;
        _outboxRepository = outboxRepository;
        _assemblyResolver = assemblyResolver;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_commited)
        {
            throw new Exception("UoW can not be commited twice within a scope");
        }

        var outboxMessageReferences = await DispatchDomainEventsAsync();
        await CommitDataChanges(cancellationToken);

        // send outbox messages (domain notification events) to outbox queue
        await PublishOutboxMessages(outboxMessageReferences);
    }

    private async Task CommitDataChanges(CancellationToken cancellationToken)
    {
        var trackedEntities = _cosmosEntityChangeTracker.GetTrackedEntities();

        if (trackedEntities is { Count: > 0 })
        {
            _logger.LogInformation("Start Commiting changes");

            foreach (var changedEntity in trackedEntities)
            {
                var repositoryType = typeof(ICosmosRepository<,>).MakeGenericType(changedEntity.GetType(), changedEntity.Id.GetType());
                ICosmosRepository repository = (ICosmosRepository)_serviceProvider.GetService(repositoryType);
                if (repository.TransactionalBatches is { Count: > 0 })
                {
                    var transactions = repository.TransactionalBatches;
                    foreach (var transaction in transactions)
                    {
                        var batchResponse = await transaction.Value.ExecuteAsync(cancellationToken);
                        if (!batchResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"Transaction failed with status code {batchResponse.StatusCode}");
                        }

                        repository.TransactionalBatches.Remove(transaction.Key);
                    }
                }
            }

            _logger.LogInformation($"Changes has been commited in database");
        }

        _commited = true;
    }

    private async Task PublishOutboxMessages(IEnumerable<OutboxMessageReference> outboxMessages)
    {
        if (!outboxMessages.Any())
        {
            return;
        }

        _logger.LogInformation($"Start pushing outbox messages into outbox-queue");

        var publishTasks = outboxMessages.Select(outboxMessage =>
            _messagePublisher.PublishAsync(_outboxConfig.Name, outboxMessage.AggregateId, outboxMessage));

        await Task.WhenAll(publishTasks);

        _logger.LogInformation("All outbox messages are just sent to outbox queue");
    }

    private async Task<IEnumerable<OutboxMessageReference>> DispatchDomainEventsAsync()
    {
        var trackedEntities = _cosmosEntityChangeTracker.GetTrackedEntities();
        var domainEvents = _cosmosEntityChangeTracker.GetDomainEvents();

        _logger.LogInformation($"Found {domainEvents.Count} domain events to dispatch");

        // Clear domain events early to avoid potential duplicates
        trackedEntities.ForEach(x => x.ClearDomainEvents());

        if (!domainEvents.Any())
        {
            return new List<OutboxMessageReference>();
        }

        var outboxMessageRefrences = new List<OutboxMessageReference>();

        foreach (var domainEvent in domainEvents)
        {
            try
            {
                var outboxMessage = OutboxMessageFactory.CreateFrom(domainEvent, _assemblyResolver.Resolve());

                var outboxMessageReference = new OutboxMessageReference(
                    outboxMessage.Id,
                    domainEvent.AggregateId,
                    outboxMessage.OccurredOn,
                    outboxMessage.Type);

                // Store the full message in outbox container (DB)
                await _outboxRepository.CreateAsync(outboxMessage);
                outboxMessageRefrences.Add(outboxMessageReference);

                _logger.LogInformation($"Successfully stored domain event {domainEvent.GetType().Name} in outbox");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing domain event {domainEvent.GetType().Name}");
                throw;
            }
        }

        return outboxMessageRefrences;
    }
}