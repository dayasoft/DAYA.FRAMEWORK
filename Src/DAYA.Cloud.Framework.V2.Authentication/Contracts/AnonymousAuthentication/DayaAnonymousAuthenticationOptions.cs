using Microsoft.AspNetCore.Authentication;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication
{
	public class DayaAnonymousAuthenticationOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = DayaAuthenticationSchemeNames.Anonymous;
	}
}