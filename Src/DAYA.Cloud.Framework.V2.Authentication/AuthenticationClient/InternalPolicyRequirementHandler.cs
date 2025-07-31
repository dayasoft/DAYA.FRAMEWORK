using Microsoft.AspNetCore.Authorization;

namespace DAYA.Cloud.Framework.V2.Authentication.AuthenticationClient;

internal class InternalPolicyRequirementHandler : AuthorizationHandler<InternalPolicyRequirement>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalPolicyRequirement requirement)
	{
		if (context.User.Identity == null || context.User.Identity.IsAuthenticated == false)
		{
			context.Fail();
			return Task.CompletedTask;
		}

		if (bool.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == "IsAuthorized")?.Value, out var isAuthorized) && isAuthorized)
		{
			context.Succeed(requirement);
		}
		else
		{
			context.Fail();
		}
		
		return Task.CompletedTask;
	}
}