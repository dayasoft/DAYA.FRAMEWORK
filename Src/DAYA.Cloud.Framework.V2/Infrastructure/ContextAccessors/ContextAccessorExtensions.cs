using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DAYA.Cloud.Framework.V2.Infrastructure.ContextAccessors
{
    /// <summary>
    /// Extension methods for registering context accessor services with .NET Core DI
    /// </summary>
    public static class ContextAccessorExtensions
    {
        /// <summary>
        /// Adds context accessor services to the service collection
        /// </summary>
        public static IServiceCollection AddContextAccessors(this IServiceCollection services)
        {
            // Register custom context accessors
            services.AddScoped<IUserContextAccessor, HttpContextUserAccessor>();

            return services;
        }
    }
}