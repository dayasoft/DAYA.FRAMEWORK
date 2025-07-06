using Microsoft.AspNetCore.Authorization;

namespace DAYA.Cloud.Framework.V2.Authentication.Contracts
{
	public class DayaPolicyDefinition
	{
		public string PolicyName { get; set; } = null!;
		public string AuthenticationSchemeName { get; set; } = null!;
		public Action<AuthorizationPolicyBuilder>? RequirementBuilder { get; set; }
	}
}