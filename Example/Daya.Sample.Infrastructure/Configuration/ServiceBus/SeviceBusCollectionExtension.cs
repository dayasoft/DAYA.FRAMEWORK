using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daya.Sample.Infrastructure.Configuration.ServiceBus
{
    public static class SeviceBusCollectionExtension
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceBusConnectionString = configuration.GetValue<string>("ServiceBus:Connection");
            services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));

            return services;
        }
    }
}