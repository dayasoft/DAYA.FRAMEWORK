using System;

namespace DAYA.Cloud.Framework.V2.Application.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string PartitionKey { get; set; }
    public DateTime OccurredOn { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Error { get; set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(DateTime occurredOn, string type, string data)
        : this()
    {
        this.Id = Guid.NewGuid();
        this.OccurredOn = occurredOn;
        this.Type = type;
        this.Data = data;
        this.PartitionKey = this.Id.ToString();
    }
}