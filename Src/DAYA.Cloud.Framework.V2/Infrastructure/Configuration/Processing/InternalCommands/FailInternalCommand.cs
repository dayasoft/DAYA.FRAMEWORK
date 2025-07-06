using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;

public record FailInternalCommand : Command
{
    public FailInternalCommand(string messageId, string? errorMessage)
    {
        MessageId = messageId;
        ErrorMessage = errorMessage;
    }

    public string MessageId { get; }
    public string? ErrorMessage { get; }
}