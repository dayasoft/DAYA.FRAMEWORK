using DAYA.Cloud.Framework.V2.Authentication.Authentication;
using DAYA.Cloud.Framework.V2.Authentication.Contracts;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.AnonymousAuthentication;
using DAYA.Cloud.Framework.V2.Authentication.Contracts.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Authentication.AuthenticationClient
{
    public static class DayaAuthenticationClientServiceCollectionExtension
    {
        public static void AddDayaAuthenticationClient(this IServiceCollection services, Func<DayaAuthenticationClientConfig> configurationBuilder, List<string> policyNames)
        {
            var authClientConfiguration = configurationBuilder();
            services.AddAuthentication(DayaAuthenticationSchemeNames.Internal)
                .AddScheme<DayaAuthenticationClientOptions, DayaAuthenticationClientHandler>(DayaAuthenticationClientOptions.DefaultScheme,
                    options =>
                    {
                        options.AuthenticationClientConfig = authClientConfiguration;
                    });
            services.AddScoped<IAuthorizationHandler, InternalPolicyRequirementHandler>();

            services.AddAuthentication(DayaAuthenticationSchemeNames.Anonymous)
                .AddScheme<DayaAnonymousAuthenticationOptions, DayaAnonymousAuthenticationHandler>(DayaAnonymousAuthenticationOptions.DefaultScheme, null);

            services.AddAuthorization(opts =>
            {
                foreach (var policyName in policyNames)
                {
                    opts.AddPolicy(policyName, policyBuilder =>
                    {
                        policyBuilder.AddAuthenticationSchemes(DayaAuthenticationSchemeNames.Internal);
                        policyBuilder.AddRequirements(new InternalPolicyRequirement());
                    });
                }
                opts.AddPolicy(DayaPolicyNames.AllowAnonymous, policyBuilder =>
                {
                    policyBuilder.AddAuthenticationSchemes(DayaAnonymousAuthenticationOptions.DefaultScheme);
                    policyBuilder.RequireAuthenticatedUser();
                });
            });
        }
    }
}