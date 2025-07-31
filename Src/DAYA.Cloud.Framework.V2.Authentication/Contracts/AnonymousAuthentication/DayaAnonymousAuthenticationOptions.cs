using Microsoft.AspNetCore.Authentication;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication
{
	public class DayaAnonymousAuthenticationOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = DayaAuthenticationSchemeNames.Anonymous;

		/// <summary>
		/// Gets or sets whether anonymous access is explicitly allowed.
		/// Default is false for security reasons.
		/// </summary>
		public bool AllowAnonymousAccess { get; set; } = false;

		/// <summary>
		/// Gets or sets the allowed paths for anonymous access.
		/// If empty, all paths are allowed when AllowAnonymousAccess is true.
		/// </summary>
		public HashSet<string> AllowedPaths { get; set; } = new();

		/// <summary>
		/// Gets or sets whether to log anonymous access attempts.
		/// Default is true for security monitoring.
		/// </summary>
		public bool LogAnonymousAttempts { get; set; } = true;
	}
}