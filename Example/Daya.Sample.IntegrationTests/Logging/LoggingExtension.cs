using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Daya.Sample.IntegrationTests.Logging
{
    public static class LoggingExtension
    {
        public static IServiceCollection AddIntegrationTestLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            return services;
        }
    }
}