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
			UrlEncoder encoder) : base(options, logger, encoder)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var requestPath = Context.Request.Path.Value ?? string.Empty;
			var remoteIp = Context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			// Log anonymous access attempts for security monitoring (if enabled)
			if (Options.LogAnonymousAttempts)
			{
				Logger.LogWarning("Anonymous authentication attempt from {RemoteIpAddress} for {RequestPath}", 
					remoteIp, requestPath);
			}

			// Check if anonymous authentication is explicitly enabled
			if (!Options.AllowAnonymousAccess)
			{
				Logger.LogWarning("Anonymous authentication denied - not explicitly enabled");
				return Task.FromResult(AuthenticateResult.Fail("Anonymous authentication is not enabled"));
			}

			// Check if the requested path is in the allowed paths (if paths are specified)
			if (Options.AllowedPaths.Count > 0 && !Options.AllowedPaths.Contains(requestPath))
			{
				Logger.LogWarning("Anonymous authentication denied for path {RequestPath} - not in allowed paths", requestPath);
				return Task.FromResult(AuthenticateResult.Fail($"Anonymous access not allowed for path: {requestPath}"));
			}

			// Create minimal claims for anonymous user with limited privileges
			var claims = new[]
			{
				new Claim(ClaimTypes.AuthenticationMethod, "anonymous"),
				new Claim("scope", "read-only"), // Restrict to read-only operations
				new Claim("user-type", "anonymous"),
				new Claim("auth-time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
				new Claim("remote-ip", remoteIp)
			};

			var identity = new ClaimsIdentity(claims, DayaAnonymousAuthenticationOptions.DefaultScheme);
			var principal = new ClaimsPrincipal(identity);
			var ticket = new AuthenticationTicket(principal, DayaAnonymousAuthenticationOptions.DefaultScheme);

			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}