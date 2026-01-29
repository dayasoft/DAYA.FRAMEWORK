using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;
using DAYA.Cloud.Framework.V2.DirectOperations.Configuration;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations.Exceptions;
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
    private readonly ConcurrencyRetryConfig _retryConfig;

    private bool _committed;

    public DirectUnitOfWork(
        IQueueMessagePublisher messagePublisher,
        OutboxConfig outboxConfig,
        ILogger<DirectUnitOfWork> logger,
        ICosmosEntityChangeTracker cosmosEntityChangeTracker,
        IServiceProvider serviceProvider,
        IOutboxMessageRepository outboxRepository,
        IContainerFactory containerFactory,
        IApplicationAssemblyResolver assemblyResolver,
        ConcurrencyRetryConfig retryConfig = null)
    {
        _messagePublisher = messagePublisher;
        _outboxConfig = outboxConfig;

        _logger = logger;
        _cosmosEntityChangeTracker = cosmosEntityChangeTracker;
        _serviceProvider = serviceProvider;
        _outboxRepository = outboxRepository;
        _assemblyResolver = assemblyResolver;
        _retryConfig = retryConfig ?? new ConcurrencyRetryConfig();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_committed)
        {
            throw new Exception("UoW can not be committed twice within a scope");
        }

        var outboxMessageReferences = await DispatchDomainEventsAsync();
        await CommitDataChangesAsync(cancellationToken);

        // send outbox messages (domain notification events) to outbox queue
        await PublishOutboxMessages(outboxMessageReferences);
    }

    private async Task CommitDataChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await CommitTransactionAsync(cancellationToken);
            return; // Success - exit retry loop
        }
        catch (ConcurrencyException ex)
        {
            _logger.LogWarning($"Concurrency conflict detected. Refreshing entities and retrying. Error: {ex.Message}");

            // Clear current transactions to start fresh
            ClearAllTransactions();

            // All retries exhausted
            throw new ConcurrencyException($"Failed to commit after 2 retries due to concurrency conflicts. This typically indicates high contention on the same document.");
        }
    }

    private async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        var trackedEntities = _cosmosEntityChangeTracker.GetTrackedEntities();

        if (trackedEntities is { Count: > 0 })
        {
            _logger.LogInformation("Start Committing changes");

            foreach (var changedEntity in trackedEntities)
            {
                var repositoryType = typeof(ICosmosRepository<,>).MakeGenericType(changedEntity.GetType(), changedEntity.Id.GetType());
                ICosmosRepository repository = (ICosmosRepository)_serviceProvider.GetService(repositoryType);
                if (repository.TransactionalBatches is { Count: > 0 })
                {
                    var transactions = repository.TransactionalBatches;
                    foreach (var transaction in transactions)
                    {
                        foreach (var batch in transaction.Value.Batches)
                        {
                            var batchResponse = await batch.ExecuteAsync(cancellationToken);
                            if (!batchResponse.IsSuccessStatusCode)
                            {
                                if (batchResponse.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                                {
                                    throw new ConcurrencyException($"Concurrency conflict detected. The document has been modified by another process. StatusCode: {batchResponse.StatusCode}");
                                }

                                throw new Exception($"Transaction failed with status code {batchResponse.StatusCode}");
                            }
                        }

                        repository.TransactionalBatches.Remove(transaction.Key);
                    }
                }
            }

            _logger.LogInformation($"Changes has been committed in database");
        }

        _committed = true;
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

        var outboxMessageReferences = new List<OutboxMessageReference>();

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
                outboxMessageReferences.Add(outboxMessageReference);

                _logger.LogInformation($"Successfully stored domain event {domainEvent.GetType().Name} in outbox");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing domain event {domainEvent.GetType().Name}");
                throw;
            }
        }

        return outboxMessageReferences;
    }

    private void ClearAllTransactions()
    {
        var trackedEntities = _cosmosEntityChangeTracker.GetTrackedEntities();

        foreach (var entity in trackedEntities)
        {
            var repositoryType = typeof(ICosmosRepository<,>).MakeGenericType(entity.GetType(), entity.Id.GetType());
            if (_serviceProvider.GetService(repositoryType) is ICosmosRepository repository)
            {
                repository.TransactionalBatches.Clear();
            }
        }
    }

    private int CalculateDelay(int attempt, Random random)
    {
        // Exponential backoff: baseDelay * (2 ^ attempt)
        var exponentialDelay = _retryConfig.BaseDelayMs * (int)Math.Pow(2, attempt - 1);

        // Cap at maximum delay
        exponentialDelay = Math.Min(exponentialDelay, _retryConfig.MaxDelayMs);

        // Add jitter to prevent thundering herd
        if (_retryConfig.EnableJitter)
        {
            var jitter = random.Next(0, exponentialDelay / 4); // Up to 25% jitter
            exponentialDelay += jitter;
        }

        return exponentialDelay;
    }
}