using Microsoft.AspNetCore.Authentication;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.ApiKeyAuthentication;

public class DayaApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
	public const string DefaultScheme = DayaAuthenticationSchemeNames.ApiKey;
	public const string TokenHeaderName = "x-api-key";
	public const string TokenQueryParameterName = "api-key";
}