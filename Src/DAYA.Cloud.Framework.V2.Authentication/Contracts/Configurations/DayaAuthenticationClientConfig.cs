namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations
{
    public class DayaAuthenticationClientConfig
    {
        public const string Key = "DayaAuthenticationClientConfig";
        public string InternalAuthenticationUrl { get; set; } = null!;
        public string SubscriptionKey { get; set; } = null!;
    }
}
