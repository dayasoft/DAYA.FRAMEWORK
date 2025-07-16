using Azure.Monitor.OpenTelemetry.AspNetCore;
using Daya.Sample.API.Configuration.Logging;

namespace Daya.Sample.API.Configuration.Logging
{
    public static class LoggingExtension
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, IHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });
            }
            else
            {
                services.AddOpenTelemetry().UseAzureMonitor();
            }

            return services;
        }
    }
}