namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.ApiKeyAuthentication
{
	public interface IApiKeyStore
	{
		public Task<List<DayaClaimItem>> GetClaimsByApiKeyAsync(string apiKey);
	}
}