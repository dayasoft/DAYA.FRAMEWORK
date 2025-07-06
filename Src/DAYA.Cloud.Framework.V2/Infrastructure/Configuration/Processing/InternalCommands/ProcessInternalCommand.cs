using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace DAYA.Cloud.Framework.V2.Infrastructure.Configuration.Processing.InternalCommands;

public record ProcessInternalCommand : Command
{
    public ProcessInternalCommand(string messageId)
    {
        MessageId = messageId;
    }

    public string MessageId { get; }
}