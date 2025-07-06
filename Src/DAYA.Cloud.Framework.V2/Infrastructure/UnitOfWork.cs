using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.InternalCommands;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.AzureServiceBus;
using DAYA.Cloud.Framework.V2.Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ServiceDbContext _context;
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;
    private readonly IQueueMessagePublisher _messagePublisher;
    private readonly OutboxConfig _outboxConfig;
    private readonly InternalCommandConfig _internalCommandConfig;
    private readonly ILogger _logger;
    private bool _commited;

    public UnitOfWork(ServiceDbContext context,
        IDomainEventsDispatcher domainEventsDispatcher,
        IQueueMessagePublisher messagePublisher,
        OutboxConfig outboxConfig,
        InternalCommandConfig internalCommandConfig,
        ILogger logger)
    {
        _context = context;
        _domainEventsDispatcher = domainEventsDispatcher;
        _messagePublisher = messagePublisher;
        _outboxConfig = outboxConfig;
        _internalCommandConfig = internalCommandConfig;
        _logger = logger;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Commiting changes...");
        if (_commited)
        {
            throw new Exception("UoW can not be commited twice within a scope");
        }

        var internalCommands = GetInternalCommands();

        var outboxMessages = await _domainEventsDispatcher.DispatchEventsAsync();
        var changes = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"{changes} changes just commited.");

        PublishOutboxMessages(outboxMessages);
        PublishInternalCommands(internalCommands);

        _commited = true;
        return changes;
    }

    private void PublishInternalCommands(IEnumerable<InternalCommandMessage> internalCommands)
    {
        if (!internalCommands.Any())
        {
            return;
        }
        _logger.LogInformation($"sending message to internalCommands queue({internalCommands.Count()})...");
        internalCommands
            .ToList()
            .ForEach(x => _messagePublisher.PublishAsync(
                _internalCommandConfig.QueueName,
                x.SessionId ?? "internalCommandsSession",
                x));
        _logger.LogInformation("message are just sent to internalCOmmands queue");
    }

    private void PublishOutboxMessages(IEnumerable<OutboxMessageRefrences> outboxMessages)
    {
        if (!outboxMessages.Any())
        {
            return;
        }
        _logger.LogInformation($"sending message to outbox queue({outboxMessages.Count()})...");
        outboxMessages
            .ToList()
            .ForEach(x => _messagePublisher.PublishAsync(
                _outboxConfig.Name,
                x.AggregateId,
                x));
        _logger.LogInformation("message are just sent to outbox queue");
    }

    private IEnumerable<InternalCommandMessage> GetInternalCommands()
    {
        return _context
            .ChangeTracker
            .Entries<InternalCommandMessage>()
            .Where(x => x.State == EntityState.Added)
            .Select(x => x.Entity)
            .Select(x => new InternalCommandMessage(x.Id,
                x.Type,
                x.Data,
                x.EnqueueDate,
                x.SessionId))
            .ToArray();
    }
}