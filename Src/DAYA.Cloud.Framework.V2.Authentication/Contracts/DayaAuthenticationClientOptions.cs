using DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations;
using Microsoft.AspNetCore.Authentication;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts
{
	public class DayaAuthenticationClientOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = DayaAuthenticationSchemeNames.Internal;
		public const string TokenHeaderName = "Authorization";
		public const string ApiManagementSubscriptionKeyHeaderName = "Ocp-Apim-Subscription-Key";
		public DayaAuthenticationClientConfig AuthenticationClientConfig { get; set; } = null!;
	}
}