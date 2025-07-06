using DAYA.Cloud.Framework.V2.Authentication.Authentication;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDayaAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDayaAuthentication(
            null,
            null,
            () =>
            {
                var entraExternalIdConfig = new EntraExternalIdConfig();
                configuration.GetSection(EntraExternalIdConfig.Key).Bind(entraExternalIdConfig);
                return entraExternalIdConfig;
            },
            null,//serviceCollection => serviceCollection.AddTransient<IApiKeyStore, ApiKeyStore>(),
            new JwtBearerEvents
            {
                OnAuthenticationFailed = AuthenticationFailed
            });

        return services;
    }

    private static Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        var logger = context.HttpContext.RequestServices.GetService<ILogger>();

        if (logger != null)
        {
            if (context?.Exception?.GetType() == typeof(SecurityTokenExpiredException))
            {
                logger.LogError("Authentication Failed, Token expired");
                return Task.CompletedTask;
            }

            logger.LogError("Authentication Failed");
            logger.LogError(context?.Exception?.ToString());
            logger.LogError(context?.Result?.ToString());
        }

        return Task.CompletedTask;
    }
}