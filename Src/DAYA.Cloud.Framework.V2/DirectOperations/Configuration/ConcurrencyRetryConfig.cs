namespace DAYA.Cloud.Framework.V2.DirectOperations.Configuration;

public class ConcurrencyRetryConfig
{
    public int MaxRetries { get; set; } = 3;
    public int BaseDelayMs { get; set; } = 100;
    public int MaxDelayMs { get; set; } = 2000;
    public bool EnableJitter { get; set; } = true;
}