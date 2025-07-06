namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations
{
    public class EntraExternalIdConfig
    {
        public const string Key = nameof(EntraExternalIdConfig);

        public string? DomainName { get; set; }

        public string TenantId { get; set; } = null!;

        public string SignUpSignInPolicyId { get; set; } = null!;

        public string Audiences { get; set; } = null!;

        //public bool UseFakeAdb2c { get; set; }
    }
}