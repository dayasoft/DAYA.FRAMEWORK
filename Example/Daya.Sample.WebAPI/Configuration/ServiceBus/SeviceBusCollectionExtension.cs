using Azure.Messaging.ServiceBus;

namespace Daya.Sample.WebAPI.Configuration.ServiceBus
{
    internal static class SeviceBusCollectionExtension
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceBusConnectionString = configuration.GetValue<string>("ServiceBus:Connection");
            services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));

            return services;
        }
    }
}