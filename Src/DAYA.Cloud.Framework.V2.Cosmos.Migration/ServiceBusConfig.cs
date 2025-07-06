namespace DAYA.Cloud.Framework.V2.Cosmos.Migration
{
    public class ServiceBusConfig
    {
        public const string ServiceBus = nameof(ServiceBusConfig);

        public string Connection { get; set; }
        public string HexKey { get; set; }
        public string OutboxQueueName { get; set; }
        public string InternalCommandQueueName { get; set; }
    }
}