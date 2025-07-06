namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations
{
    public class B2CAuthenticationConfig
    {
        public const string Key = nameof(B2CAuthenticationConfig);

        public string TenantName { get; set; } = null!;

        public string DomainName { get; set; } = null!;

        public string TenantId { get; set; } = null!;

        public string DefaultPolicy { get; set; } = null!;

        public string Audiences { get; set; } = null!;

        public bool UseFakeAdb2c { get; set; }
    }
}