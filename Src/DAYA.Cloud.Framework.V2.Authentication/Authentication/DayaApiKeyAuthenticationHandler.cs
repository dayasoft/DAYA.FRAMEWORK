using System.Security.Claims;
using System.Text.Encodings.Web;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.ApiKeyAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DAYA.Cloud.Framework.V2.Authentication.Authentication
{
	public class DayaApiKeyAuthenticationHandler : AuthenticationHandler<DayaApiKeyAuthenticationOptions>
	{
		private readonly IApiKeyStore _apiKeyStore;

		public DayaApiKeyAuthenticationHandler(
			IOptionsMonitor<DayaApiKeyAuthenticationOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			IApiKeyStore apiKeyStore) : base(options, logger, encoder)
		{
			_apiKeyStore = apiKeyStore;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			try
			{
				string? apiKey = null;
				if (Request.Headers.TryGetValue(DayaApiKeyAuthenticationOptions.TokenHeaderName, out var headerToken) == true
					&& string.IsNullOrWhiteSpace(headerToken) == false)
				{
					apiKey = headerToken!;
				}
				else
				{
					var errorMessage = $"Missing API key in header: {DayaApiKeyAuthenticationOptions.TokenHeaderName}. Query parameter authentication is disabled for security reasons.";
					Logger.LogError("Api Key Authentication Failed. " + errorMessage);
					return AuthenticateResult.Fail(errorMessage);
				}

				var dayaClaims = await _apiKeyStore.GetClaimsByApiKeyAsync(apiKey);
				if (dayaClaims is not { Count: > 0 })
				{
					var errorMessage = "Invalid Api Key";
					Logger.LogError("Api Key Authentication Failed. " + errorMessage);
					return AuthenticateResult.Fail(errorMessage);
				}

				var claims = dayaClaims.Select(x => new Claim(x.Type, x.Value)).ToList();

				var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);

				var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

				return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
			}
			catch (Exception e)
			{
				Logger.LogError(e, "Api Key Authentication Failed.");
				return AuthenticateResult.Fail("Api Key Authentication Failed.");
			}
		}
	}
}