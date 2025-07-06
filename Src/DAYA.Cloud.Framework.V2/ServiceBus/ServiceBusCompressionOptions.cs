namespace DAYA.Cloud.Framework.V2.ServiceBus
{
    public class ServiceBusQueuePublisherCompressionOptions
    {
        /// <summary>
        /// Whether to enable GZip compression before sending the message.
        /// </summary>
        public bool EnableCompression { get; set; } = false;
    }
    public class ServiceBusTopicPublisherCompressionOptions
    {
        /// <summary>
        /// Whether to enable GZip compression before sending the message.
        /// </summary>
        public bool EnableCompression { get; set; } = false;
    }
}
