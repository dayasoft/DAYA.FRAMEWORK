using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;

internal class FailInternalCommandHandler : ICommandHandler<FailInternalCommand>
{
    private readonly ServiceDbContext _context;
    private readonly ILogger _logger;

    public FailInternalCommandHandler(ServiceDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(FailInternalCommand request, CancellationToken cancellationToken)
    {
        var messageId = Guid.Parse(request.MessageId);
        var internalCommand = await _context
            .InternalCommands
            .WithPartitionKey(messageId.ToString())
            .Where(x => x.Id == messageId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (internalCommand is null)
        {
            _logger.LogWarning("Internal command with ID {MessageId} not found. Skipping.", request.MessageId);
            return;
        }

        try
        {
            internalCommand.Error = request.ErrorMessage;
            internalCommand.ProcessedDate = Clock.Now;

            _context.Update(internalCommand);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Internal command with ID {MessageId} marked as failed with error: {ErrorMessage}",
                request.MessageId, request.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update internal command with ID {MessageId}.", request.MessageId);
            throw;
        }
    }
}