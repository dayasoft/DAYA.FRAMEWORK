using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.Outbox;

public record FailOutboxCommand : Command
{
    public FailOutboxCommand(string outboxMessageId, string? errorMessage)
    {
        MessageId = outboxMessageId;
        ErrorMessage = errorMessage;
    }

    public string MessageId { get; }
    public string? ErrorMessage { get; }
}