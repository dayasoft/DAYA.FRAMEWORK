using System.Security.Claims;
using System.Text.Encodings.Web;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DAYA.Cloud.Framework.V2.Authentication.Authentication
{
	public class DayaAnonymousAuthenticationHandler : AuthenticationHandler<DayaAnonymousAuthenticationOptions>
	{
		public DayaAnonymousAuthenticationHandler(
			IOptionsMonitor<DayaAnonymousAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			return Task.FromResult(AuthenticateResult.Success(
				new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(authenticationType: "Bearer")),
					DayaAnonymousAuthenticationOptions.DefaultScheme)));
		}
	}
}