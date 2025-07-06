using System;

namespace DAYA.Cloud.Framework.V2.Application.InternalCommands;

public class InternalCommandMessage
{
    public Guid Id { get; set; }
    public string PartitionKey { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime EnqueueDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Error { get; set; }
    public string? SessionId { get; set; }

    private InternalCommandMessage()
    {
    }

    public InternalCommandMessage(Guid id, string type, string data, DateTime enqueueDate, string sessionId = null)
        : this()
    {
        Id = id;
        Type = type;
        Data = data;
        EnqueueDate = enqueueDate;
        SessionId = sessionId;
        PartitionKey = id.ToString();
    }
}