using Microsoft.AspNetCore.Authorization;

public class InternalAuthenticationRequirementHandler : AuthorizationHandler<InternalAuthenticationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<InternalAuthenticationRequirementHandler> _logger;
    private readonly IConfiguration _configuration;

    public InternalAuthenticationRequirementHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<InternalAuthenticationRequirementHandler> logger,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, InternalAuthenticationRequirement requirement)
    {
        try
        {
            await TryHandleRequirementAsync(context, requirement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authorization failed.");
            context.Fail();
        }
    }

    private async Task TryHandleRequirementAsync(AuthorizationHandlerContext context, InternalAuthenticationRequirement requirement)
    {
        var httpContext = _httpContextAccessor!.HttpContext!;

        httpContext.Request.Headers.TryGetValue("internalKey", out var internalKey);
        if (internalKey.Count == 0)
        {
            _logger.LogInformation("internalKey is not provided.");
            context.Fail();
            return;
        }

        if (internalKey.Count > 1)
        {
            _logger.LogWarning("Multiple internalKey headers are provided.");
            context.Fail();
            return;
        }

        // check if the internal key value is valid
        var internalKeyStr = internalKey[0];
        if (internalKeyStr is null || internalKeyStr != _configuration["InternalAuthorizationKey"])
        {
            _logger.LogWarning("Invalid internalKey value.");
            context.Fail();
            return;
        }

        context.Succeed(requirement);
        await Task.CompletedTask;
    }
}