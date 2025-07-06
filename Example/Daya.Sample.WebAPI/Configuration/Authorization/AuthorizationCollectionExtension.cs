using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication;
using Microsoft.AspNetCore.Authorization;

public static class AuthorizationCollectionExtension
{
    public static IServiceCollection AddDayaAuthorization(this IServiceCollection services)
    {
        //services.AddScoped<IAuthorizationHandler, PlatformPermissionsRequirementHandler>();
        //services.AddScoped<IAuthorizationHandler, B2BPermissionsRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, InternalAuthenticationRequirementHandler>();

        services.AddAuthorization(opts =>
        {
            opts.AddPolicy(DayaPolicyNames.InternalService, policyBuilder =>
            {
                policyBuilder.AddAuthenticationSchemes(DayaAnonymousAuthenticationOptions.DefaultScheme);
                policyBuilder.AddRequirements(new InternalAuthenticationRequirement());
            });

            opts.AddPolicy(DayaPolicyNames.EntraExternalIdOnly, policyBuilder =>
            {
                policyBuilder.AddAuthenticationSchemes(DayaAuthenticationSchemeNames.EntraExternalId);
                policyBuilder.RequireAuthenticatedUser();
            });
        });

        return services;
    }
}