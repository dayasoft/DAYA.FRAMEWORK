using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox;

internal class FailOutboxCommandHandler : ICommandHandler<FailOutboxCommand>
{
    private readonly ServiceDbContext _context;
    private readonly ILogger _logger;

    public FailOutboxCommandHandler(ServiceDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(FailOutboxCommand request, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(request.MessageId);
        var outboxMessage = await _context
            .OutboxMessages
            .WithPartitionKey(messageId.ToString())
            .Where(x => x.Id == messageId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (outboxMessage is null)
        {
            _logger.LogWarning("Outbox message with ID {MessageId} not found. Skipping.", request.MessageId);
            return;
        }

        try
        {
            outboxMessage.Error = request.ErrorMessage;
            outboxMessage.ProcessedDate = Clock.Now;

            _context.Update(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Outbox message with ID {MessageId} marked as failed with error: {ErrorMessage}",
                request.MessageId, request.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update outbox message with ID {MessageId}.", request.MessageId);
            throw;
        }
    }
}